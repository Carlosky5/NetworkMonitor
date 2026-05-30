# Developer Notes — Borderless Window Mechanics and Graphy Control

This file documents the non-obvious traps in this codebase. It is written for someone (possibly a future AI session) who already understands .NET WinForms but has not worked with this specific project before. Read this before touching anything related to window movement, resizing, the ColourChanger thread, or the Graphy graph control.

---

## 1. Win32 `SendMessage` requires an explicit alias

**Symptom:** `EntryPointNotFoundException` at runtime when calling `SendMessage`.

**Cause:** `user32.dll` does not export a plain `SendMessage` symbol. It only exports `SendMessageA` (ANSI) and `SendMessageW` (Unicode). The VB.NET P/Invoke declaration must include an explicit alias:

```vb
Declare Function SendMessage Lib "user32" Alias "SendMessageW" (...)
```

Omitting `Alias "SendMessageW"` compiles fine but throws at the first call.

---

## 2. Window movement must be done manually — do NOT use `WM_MOVING` + `SendMessage(HTCAPTION)`

**Symptom:** Snap-to-edge works on the first drag but not on any subsequent drag. Or the window locks permanently to a corner. Or it jumps across the screen when crossing monitor boundaries.

**Root cause:** The approach of handling `WM_MOVING` in `WndProc` and calling `SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0)` to hand off dragging to Windows is fundamentally unreliable for a borderless form with child controls, for several reasons:

- `WM_ENTERSIZEMOVE` (which is the only safe place to snapshot the grab offset) may fire *after* the first `WM_MOVING` message in some configurations, meaning the grab offset is stale for the first event.
- Once you snap the window in `WM_MOVING` by modifying the `RECT` in `lParam`, Windows recalculates the drag anchor internally, and subsequent messages carry a corrupt relative position. This causes the window to "chase" the cursor rather than track it.
- Crossing monitor boundaries triggers `WM_MOVING` relative to the new screen's coordinate space mid-drag, which interacts badly with snap logic written for the previous screen.

**The working solution** (what this codebase uses): Abandon `WM_MOVING` for move entirely. Track movement manually with `MouseDown` / `MouseMove` / `MouseUp`:

- On `MouseDown`: record `_dragGrabX = Cursor.Position.X - Left` and `_dragGrabY = Cursor.Position.Y - Top`. Set `_move = True`.
- On `MouseMove`: if `_move`, compute `Left = Cursor.Position.X - _dragGrabX` and `Top = Cursor.Position.Y - _dragGrabY`, then apply snap logic.
- On `MouseUp`: set `_move = False`, save registry.

The grab offset must be captured **once at mouse-down time** and never recomputed during the drag. If you recompute it mid-drag (e.g., inside `WM_MOVING`), snapping will corrupt the offset and the window will get stuck.

**Resize is different:** Horizontal resize *does* use `SendMessage(Handle, WM_NCLBUTTONDOWN, HTLEFT/HTRIGHT, 0)` and this works correctly because Windows handles the resize loop without the application needing to track position. `WM_ENTERSIZEMOVE` and `WM_EXITSIZEMOVE` are still useful for showing the colour animation during resize.

---

## 3. Child controls intercept mouse events — WndProc alone is not enough

**Symptom:** Resize cursor and drag do not activate when the mouse is over a label or panel — only over the bare form background.

**Cause:** Mouse events on child controls are handled by those controls, not the form. `WndProc` on the form only sees `WM_NCHITTEST` for the form's own non-client and client area. When the mouse is over a `Label`, the label gets the hit test and the form never sees it.

**Solution:** Attach `MouseDown`, `MouseMove`, `MouseUp`, `MouseClick`, `MouseEnter`, and `MouseLeave` handlers to every control recursively via `GetAllControls`. All handlers call the same "Everything_" methods. The resize logic in `Everything_MouseDown` uses `Cursor.Position.X` (screen coordinates) rather than the sender's local coordinates, so it works identically regardless of which control was the event source.

---

## 4. Snap-to-edge logic: two-sided zone, grab-offset-aware

The snap zone is centred on the cursor position where the relevant window edge *would* land flush with the screen edge — not on the screen edge itself. This matters because the grab point inside the window is not necessarily the edge.

```
Left snap fires when:
  cursor.X is within ±_autoSnapDistance of (workingArea.Left + _dragGrabX)
```

A one-sided check (`cursor.X < workingArea.Left + _autoSnapDistance`) causes the window to lock: once snapped, the condition stays true and the window can never be unsnapped by dragging away from the edge.

The zone must be two-sided (`x < zone + distance AndAlso x > zone - distance`) so that dragging the cursor far enough in either direction exits the zone and releases the snap.

---

## 5. `ColourChanger` threading rules

`ColourChanger` runs on a `ThreadPool` thread. Several invariants must be maintained:

- **Only one instance must run at a time.** Use `Interlocked.CompareExchange(_colourChangerRunning, 1, 0)` before queuing. If the exchange returns 1, a thread is already running — do not queue another.
- **Clear the running flag *after* the cleanup `Invoke`, not before.** Clearing the flag before the cleanup `Invoke` opens a window where a new drag can queue a second `ColourChanger` that starts before the first has restored the background colour. The order must be: `Invoke` (restore colour, show panel, hide label) → `Interlocked.Exchange(_colourChangerRunning, 0)`.
- **`BeginInvoke` for the colour loop, `Invoke` for cleanup.** During the colour cycle, `BeginInvoke` is fine (fire-and-forget colour updates). For the final restoration, `Invoke` (blocking) is used so the flag is not cleared until the UI has actually applied the changes.
- **Guard against `ObjectDisposedException`.** If the form is closed while `ColourChanger` is running, the `Invoke` will throw. Wrap it in a try/catch and check `_cts.IsCancellationRequested` before invoking.

---

## 6. NetworkLoop shutdown

`NetworkLoop` runs on a `ThreadPool` thread and loops indefinitely. On form close:

1. `OnFormClosed` calls `_cts.Cancel()` **first**, before disposing anything.
2. The loop condition `While Not _cts.IsCancellationRequested` causes the loop to exit on the next iteration.
3. `BeginInvoke` inside the loop checks `_cts.IsCancellationRequested` before posting UI updates.
4. `ObjectDisposedException` from `BeginInvoke` is caught and causes an immediate `Return`.
5. The exception filter `Catch ex As Exception When Not _cts.IsCancellationRequested` prevents cancelled-shutdown exceptions from being re-logged and re-spawning the loop.

If you add any `Invoke` or `BeginInvoke` calls inside `NetworkLoop`, they must all be guarded the same way.

---

## 7. `StatsSync.Lock` scope

The lock exists to protect shared stat fields (`Download.*`, `Upload.*`) that are written by `NetworkLoop` and read by the UI thread and the TTS thread. Keep the lock scope minimal:

- Read values into locals under the lock, then do all computation and UI work *outside* the lock.
- Never call `Invoke` / `BeginInvoke` while holding the lock — that can deadlock if the UI thread is waiting on the lock.
- `RefreshLabels` and `RefreshGraphs` are called on the UI thread via `BeginInvoke`, so by the time they run, the NetworkLoop thread may have already updated the stats again. This is intentional and acceptable — we display whatever is current at paint time.

---

## 8. Adapter enumeration is throttled

`NetworkInterface.GetAllNetworkInterfaces()` is an expensive kernel call. It runs at most once every 10 seconds (`_adapterCheckCountdown`). Between checks, the loop assumes the cached `ActiveInterface` is still valid and reads `GetIPv4Statistics()` directly.

If the active adapter disappears (the loop detects this when `networkIDsMatched` is false at check time), it re-enumerates immediately and resets session stats. If you change this logic, make sure adapter loss still triggers an immediate re-enumeration rather than waiting for the next countdown expiry.

---

## 9. Graphy dependency — what it is and how the DLL flows in

NetworkMonitor displays live network speed graphs using **[Graphy](https://github.com/Carlosky5/Graphy)**, a custom WinForms `UserControl` maintained as a separate project. Understanding the dependency structure is important before touching anything graph-related.

### How the DLL gets into NetworkMonitor

Graphy is **not** referenced via `<ProjectReference>`. It is a pre-built binary committed to this repo at `lib\Graphy.dll`. The `.vbproj` references it with a relative HintPath:

```xml
<Reference Include="Graphy">
  <HintPath>..\lib\Graphy.dll</HintPath>
</Reference>
```

Do not switch to a ProjectReference. A cross-solution ProjectReference between a C# project and a VB.NET project generates a machine-specific absolute path that breaks clones.

### Updating Graphy

If you change Graphy's source:

1. Build Graphy in **Debug** configuration (not Release — see Graphy's DEVNOTES section 2)
2. Copy `Graphy\bin\Debug\Graphy.dll` to `NetworkMonitor\lib\Graphy.dll`
3. If you changed the public API, update `MainForm.Designer.vb` to match
4. Build NetworkMonitor to verify, then commit both the DLL and any code changes together

### Graphy configuration in MainForm.Designer.vb

Two Graphy instances are configured at design time:

- **GraphyDL** (download): `LineColour = Green`, `OverlayText = "DB: "`, `PaddingHeight = 0.9`
- **GraphyUL** (upload): `LineColour = RoyalBlue`, `OverlayText = "UB: "`, `PaddingHeight = 0.9`

Both use `GraphType.Gradient`, `ShowTextOverlay = True`, and `IndexIndicatorColour = White`.

---

## 10. Graphy control — graph rendering traps

The `Graphy.dll` in `lib/` is a custom WinForms `UserControl` (source in the sibling `Graphy/` project). Several non-obvious issues exist in its rendering pipeline.

### 10a. Graph must not draw into the overlay text area

**Symptom:** The graph line and fill render on top of or directly behind the "DB: / UB:" overlay text in the top-left corner, making the text hard to read.

**Cause:** If graph points are computed using `y = Height * (1 - ratio * PaddingHeight)`, the topmost point sits at `Height * (1 - PaddingHeight)` pixels from the top of the control. With `PaddingHeight = 0.9` and `Height = 100`, that is only 10 px — the exact height of the overlay text at 7pt Arial. Any anti-aliasing bleed or gradient colour from the fill makes the overlap visible even when the line itself is technically below the text.

**Fix:** Compute a `topMargin` from the actual font height (`_overlayFont.Height + 2`), then map all graph points into the range `[topMargin, Height]`:

```csharp
float topMargin   = ShowTextOverlay ? _overlayFont.Height + 2f : 0f;
float graphHeight = Height - topMargin;
y = topMargin + graphHeight * (1f - ratio * PaddingHeight);
```

The gradient fill brush must also be anchored to `topMargin → Height`, not `0 → Height`, otherwise the gradient accumulates visible colour in the text zone even though no polygon fill is drawn there.

### 10b. `FillPolygon` replaces per-column `DrawLine` for gradient/fill modes

The original code drew one vertical `DrawLine` per pixel column using a freshly allocated `LinearGradientBrush` and `Pen` — hundreds of GDI object pairs per frame, none disposed. The replacement is a single `FillPolygon` call with a cached brush:

- Build a `PointF[count + 2]` polygon: the data line points, then bottom-right corner `(lastX, Height)`, then bottom-left corner `(firstX, Height)`.
- Fill with a vertical `LinearGradientBrush` spanning `topMargin` (transparent) to `Height` (semi-opaque).

This reduces the fill from `Width` draw calls to 1 per frame.

### 10c. `DrawLines` must receive exactly `count` points — no extras

`Graphics.DrawLines` draws lines between **all** elements in the array it receives. If the reusable `_pointsBuffer` has `Length > count` (e.g., allocated at a previous larger size), the extra slots contain default `PointF(0, 0)` values, causing phantom lines to the top-left corner of the control.

**Fix:** Pass only the filled slice. If `_pointsBuffer.Length == count`, pass it directly. Otherwise copy the first `count` elements:

```csharp
PointF[] drawPoints = count == _pointsBuffer.Length
    ? _pointsBuffer
    : GetSlice(_pointsBuffer, count);
g.DrawLines(_linePen, drawPoints);
```

The buffer only ever grows (never shrinks mid-session), so after the warmup phase it equals `count` exactly and no copy is needed.

### 10d. All GDI resources in the Graphy control must be cached

The control creates `LinearGradientBrush`, `Pen`, `SolidBrush`, and `Font` objects. Every one of these must be cached as a field and only recreated when their inputs change (`LineColour`, `Height`, `topMargin`, `OverlayColour`, `IndexIndicatorColour`, `Parent.BackColor`). Creating them fresh inside `Paint` — especially inside a per-pixel loop — causes hundreds of unmanaged GDI handle allocations per second.

All cached resources are disposed in `DisposeGdiResources()`, called from the Designer-generated `Dispose(bool)` override.
