
# Changelog

## [1.0.0] - Initial Release

**Release Date:** May 2026

### Overview

Initial release of the **SS.TweenAnimations** package. A highly optimized, Zero-GC, Data-Oriented animation engine built natively for Unity. Designed to bypass the standard `MonoBehaviour` update cycle via C++ PlayerLoop injection, ensuring maximum CPU cache hits and zero runtime allocations.

### Added

#### Core Engine & Architecture

* **Zero-GC Execution Engine:** Implemented a contiguous array-based routing system (`TweenEngine`) that entirely avoids virtual method overhead and garbage collection during per-frame updates.
* **Native PlayerLoop Injection:** Engine directly integrates into Unity's native `Update` subsystem, completely bypassing expensive `Update()` reflection calls.
* **O(1) Memory Management:** Implemented a high-speed "Swap and Pop" algorithm for destroying completed tweens without shifting array indexes or leaking memory.
* **Struct-Based Data (`TweenState`):** All animation states are stored as unboxed value types, guaranteeing absolute zero memory allocation during runtime execution.

#### Editor & Workflow Tools

* **Polymorphic Inspector UI:** Developed a highly advanced Custom Property Drawer (`TweenConfigDrawer`) utilizing `[SerializeReference]` for hot-swapping animation configs and logic curves directly in the Inspector.
* **Context-Aware Filtering:** The Inspector intelligently filters and displays only the mathematical curves that belong to the currently selected animation type (e.g., preventing Move curves from being assigned to Rotate configs).
* **Dynamic Visual Hierarchy:** The UI automatically renders sleek dividers, headers, and hides irrelevant parameters (like Loop Count when looping is disabled) for a clean designer experience.
* **Tween Animation Factory (Code Generator):** Added an Editor Window tool (`SudStack -> TweenAnimation -> Create New Animation`) to instantly scaffold and generate C# boilerplate for new animation configs and mathematical logic curves.

#### Animation Modules

* **Move (Translation):** Local and World space position tweening.
* **Scale:** Local scale transformations.
* **Rotate:** Local and World space Euler angle rotations.
* **UI Fill:** Optimized `fillAmount` manipulation for Unity UI Images.
* **UI Chasing Loader:** A complex, dual-mutation (Rotation + Sine Wave Fill) animation for creating organic, Material-design style indeterminate loading spinners.

#### Mathematical Easing Library

* Integrated a stateless, centralized `EaseUtility` featuring industry-standard Robert Penner equations:
* Linear
* Ease In
* Ease Out
* Ease In Out
* Bounce Out
* Elastic Out
* Sine Pulse (specifically for breathing UI elements)



#### Looping & Lifecycle

* **Robust Looping System:** Added support for `Restart`, `PingPong`, and `None`.
* **Infinite Looping:** Support for continuous execution by setting the loop count to `-1`.
* **Frame-Perfect Restarts:** Built-in fractional time compensation to prevent frame drift during continuous looping.

#### Event & Callback System

* **Zero-GC Interface Callbacks:** Added `ITweenListener` for receiving animation completion events completely free of lambda/delegate allocations.
* **UnityEvent Bridge:** Added `TweenCallbackEvents` component to easily map interface callbacks to designer-friendly UnityEvents (supports passing the animated `Transform`).
