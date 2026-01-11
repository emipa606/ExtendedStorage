# GitHub Copilot Instructions for RimWorld Mod: Extended Storage (Continued)

## Mod Overview and Purpose
Extended Storage (Continued) is a mod for RimWorld that enhances the game's storage system by introducing additional storage buildings. These buildings can multi-stack items, allowing players to store more materials efficiently without cluttering their base. The mod operates by using an input cell and a storage cell; items are stacked from the input cell to the storage cell until the internal limit is reached, thus optimizing the storage capacity.

## Key Features and Systems
- **Multi-Stack Storage Buildings:** Introduces various new storage buildings, each with specific storage limits and capabilities for different resource types.
- **Seamless Pawn Interaction:** Pawns recognize these storage setups and interact with them naturally, such as retrieving and depositing items as needed.
- **Dynamic Storage Controls:** Utilizes RimWorld's existing stockpile mechanics to determine what can be stored in each building through configurable thing categories.
- **User-Friendly Mod Usage:** No external dependencies required; simply subscribe and activate the mod.

## Coding Patterns and Conventions
- **Class Naming:** Each class is prefixed with its functional context, e.g., `Building_ExtendedStorage` for storage buildings.
- **Accessibility Modifiers:** Internal classes and methods are widely used, promoting encapsulation while exposing essential components.
- **Descriptive Method Names:** Methods are named to clearly reflect their functionality, e.g., `TryGrabOutputItem`, `InvalidateThingSection`.
- **Modular Design:** Code is modular, with specific responsibilities assigned to different classes to ensure maintainability and scalability.

## XML Integration
- **XML Definitions:** Storage buildings and items are defined and configured using XML to dictate properties such as storage limits and accepted items.
- **Thing Categories:** XML is utilized to specify thing categories that storage buildings interact with, allowing for flexible storage configurations similar to stockpiles.

## Harmony Patching
- **Patch Strategy:** Harmony is employed to patch existing RimWorld methods to modify or extend base game functionality as needed.
- **Targeted Patching:** Each Harmony patch targets specific methods without altering the core game systems unnecessarily, ensuring compatibility.

## Suggestions for Copilot
- **Utilize Consistent Naming Conventions:** Ensure generated code uses established naming conventions such as `Try`, `Notify`, and `Recalculate` prefixes for methods performing specific actions.
- **Leverage Harmony for Integrations:** When extending RimWorld functions, consider using Harmony patches to inject custom behaviors without overwriting existing methods.
- **Promote Code Modularity:** Suggest creating helper methods and new classes for distinct functionalities to maintain clean and organized code.
- **XML Integration Considerations:** Ensure Copilot recommends XML structures that conform to RimWorld's stockpile and storage definitions for consistency and functionality.
- **Debugging and Testing**: Recommend logging and debug outputs where necessary, given RimWorld’s complexity and the need for visibility into mod behavior.

By following these guidelines, contributors can effectively develop new features and maintain the Extended Storage mod, ensuring a seamless and enhanced gameplay experience for all users.
