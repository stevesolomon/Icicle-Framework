Icicle-Framework
================

A Component-Based Object Composition Game Framework for XNA

*What is Component-Based Object Composition?*

CBOC moves away from the standard object hierarchies present in traditional OO code and looks at treating an object as nothing more than a set of components. The base object in this case, has little to no functionality itself. Instead, we add components to it in order to add the desired functionality.

In effect, this allows you to strip away the traditional deep hierarchy of 'game objects' and be left with only a single class in the game object hierarchy: GameObject. GameObject acts as a storage container for components and you add components as desired to each _instance_ of a GameObject in order to provide that GameObject with the necessary functionality.

*How does Icicle Framework handle this?*

We have our base game object class, aptly named as "GameObject". It contains only three parameters that I believe are fundamental to all game objects no matter what each may be used for (and, thus, are included within GameObject rather than components): a unique identifier  (stored as a GUID), a record of whether or not this GameObject is active, and the position of the GameObject. 

The GameObject class serves its purpose as a container for components that add functionality in one way or another. You retrieve/query components via the generic `T GetComponent<T>()` method, where `T` is the base interface of your component (which, further, must implement `IBaseComponent`). You add components via `void AddComponent(Type baseInterfaceType, IBaseComponent component)` method. In this case, `baseInterfaceType` must be that same base interface for your component that you would expect to have to use when retrieving it via `GetComponent`. 

The framework supports loading GameObject (and associated component) definitions from XML. I believe this is an ideal method, as it lets you take advantage of data-driven design - no compilation required to modify the basic fundamentals of your GameObjects. Currently, custom (ie: written by hand) deserialization methods are used to instantiate objects from XML. Built-in/language-provided serialization will be considered and implemented at a future point in time when more user-friendly methods exist to define GameObjects and components (ie: a GUI). An editor/GUI for GameObjects is not currently planned, however, so such functionality will come at a later, unknown point in time.

----

*New Features*

(1) Pooling has been implemented for all GameObjects, Behaviors, and Actions (Renderables are not currently pooled). 

(2) Farseer support has been limited to Bodies and World simulation only. The existing Joint code remains but has been too costly to maintain and improve. (Not exactly a new feature but...)

(3) Behavior/Action component system replaces confusing and overly complex subcomponents, and mitigates the explosion of component types that may occur when developing a functional game. Behaviors allow for arbitrary responses to internal or external events/stimuli, and Actions allow for reactions to behaviors (or other sources). This functionality marks a major milestone for Icicle Framework.

(4) Metadata component removed in favor of pushing metadata to the GameObject level. The slightly increased complexity was determined to be worthwhile due to the ease of use offered by this new solution, along with the fact that many, if not most, GameObjects will have at least one element of metadata associated with them. Metadata is similar to 'tags' in Unity 3D, only in the case of Icicle Framework a GameObject may have multiple elements of metadata, and each piece of metadata may store an accompanying object.

*Current To-Do List*

(1) Develop new example game, deprecate existing Breakout clone which relies heavily on Farseer (deprecated).

(2) Improve particle manager, which ties in which Mercury Particle Engine.

(3) Develop new input handling system based around 'commands'. A single command represents a series of buttons/keys that may be pressed along varying units of time. This will allow complex button commands to be possible within the framework itself, and will be fun to write!

(4) Consider developing a GUI for GameObject/Component creation, to reduce tedium and possibility of errors when hand-crafting XML definitions.

