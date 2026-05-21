# TweenAnimation

The package provides an ultra-high-performance, modular, and allocation-free tweening architecture. Engineered with Data-Oriented Design (DOD) principles, this engine leverages direct **PlayerLoop Injection** and a strict **Zero-GC runtime cycle**. this package **does not** rely on MonoBehaviours, Coroutines, or lambda expressions, this engine completely eliminates the architectural baggage of continuous Garbage Collection (GC) and CPU overhead. It completely bypasses traditional Unity update loops, ensuring buttery-smooth frame rates without memory spikes, while still providing a highly flexible and designer-friendly interface in the Unity Inspector.

---

## 💎 Core Features

* **Absolute Zero-GC Runtime:** The execution loop generates zero bytes of garbage. Active animations are treated as pure value-type structs (`TweenState`) managed within pre-allocated contiguous arrays.
* **Direct PlayerLoop Injection:** The engine injects its calculation ticks directly into Unity's native internal execution cycle, yielding maximum speed and bypassing the overhead of standard Monobehaviours.
* **Polymorphic Inspector UI:** Utilizing advanced serialization, designers are given a clean, dynamic dropdown in the Inspector. They can hot-swap animation behaviors (e.g., Move, Fade, Scale) instantly without changing the underlying scene structure.
* **O(1) Memory Maintenance (Swap & Pop):** When animations finish, the engine removes them using a high-speed Swap & Pop algorithm. This prevents the heavy loop-shifting overhead associated with standard lists, keeping CPU execution completely flat.
* **Designer-Friendly Event Bridge:** A completely modular, Zero-GC callback system. Designers can use a dedicated Inspector component (similar to Unity UI Buttons) to trigger audio, particles, or other scripts upon animation completion, requiring zero programming knowledge.

---

## 📂 Folder Structure Guide

The package follows strict Unity Package Manager (UPM) architectural standards, isolating responsibilities to maintain a scalable and clean codebase.

```text
com.sudstack.tweenanimation/
│
├── package.json                  # UPM metadata (Version, Author, Platform rules)
├── README.md                     # Deep technical documentation
│
├── Editor/                       # Scripts isolated strictly for the Unity Editor
│   ├── TweenAnimation.Editor.asmdef
│   └── PropertyDrawers/          # Houses the custom inspector rendering logic
│
└── Runtime/                      # Core game code compiled into the final build
    ├── TweenAnimation.Runtime.asmdef
    │
    ├── Core/                     # The Engine's Brain (Zero-GC Execution Hub)
    │   # Contains the main engine array, PlayerLoop injector, state structs, and math utilities.
    │
    ├── Interfaces/               # Architectural Contracts
    │   # Defines the strict rules for configs, logic routing, and memory-safe callbacks.
    │
    ├── Configs/                  # Data Containers (What the designer sees)
    │   # Houses the master polymorphic base class and individual property data (e.g., Move settings).
    │
    ├── Animations/               # Execution Logic (The math and movement)
    │   # Contains the isolated mathematical ticks and Inspector logic dropdown definitions.
    │
    └── Components/               # Monobehaviours for the Scene
        # Contains the Event Bridge script that designers attach to GameObjects for callbacks.

```

---

## 🛠️ Extensibility: How to Add New Animations

Because the architecture is fully decoupled, **you never need to modify the core engine files** to add new animation capabilities (like Scale, Rotate, or Color Fade).

To add a new animation type, you simply need to create two conceptual files in their respective folders:

### Step 1: Create the Data Config (The Inspector UI)

1. Navigate to the **Configs** folder and create a new class (e.g., `ScaleTweenConfig`).
2. Make it inherit from the engine's master `TweenConfigBase`.
3. Define the specific variables you want the designer to see in the Inspector (like a target scale value).
4. Override the base method responsible for applying custom values. Inside this method, simply grab the GameObject's current scale as the starting value, and assign your designer's target scale as the ending value.

### Step 2: Create the Animation Logic (The Math & Execution)

1. Navigate to the **Animations** folder and create a new logic script.
2. Inside this script, you will create two things: an Inspector configuration mapping and the actual math loop.
3. **The Config Mapping:** Create a class that implements the engine's logic interface. This simply defines the name that will appear in the Inspector's "Curve/Logic" dropdown and points the engine to your math function.
4. **The Math Function:** Create a static class with a specific method that calculates the animation frame.
5. In this static class, use Unity's runtime initialization attribute to automatically register your math function into the core engine when the game boots.
6. Inside your calculation method, read the time progress, pass it through the engine's centralized math utility for smoothing (easing), and apply the final calculated value directly to the target GameObject's transform.

**Pro-Tip on Reusability**: You do not always have to write new math from scratch! Because the system is highly modular, you can easily pair a brand-new Data Config with the mathematical logic files we have already created. You can route new animations to leverage our existing, centralized easing utilities and pre-built logic curves, saving development time and keeping the project incredibly clean.


---

Built for high performance, this Tween Animation engine provides the perfect balance between hardcore optimization and designer accessibility. Drop it into your project, assign your configs, and watch your UI and game objects animate at maximum engine efficiency without dropping a single frame. Happy developing!