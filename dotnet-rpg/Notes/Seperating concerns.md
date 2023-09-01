# How to organise the separation of concerns better?

**Feature: addition of `Skills` to `Characters`**

## What service to use for this?

The division of responsibilities between services can often be seen as a design choice, and there can be some
flexibility depending on your specific use case and project requirements.

- In this scenario, the SkillService could certainly be designed to handle the addition of Skills to a Character, but it
  might lead to **tight coupling between the Skill and Character entities**, which can make the system more difficult to
  maintain and extend in the future.
- By adhering to the **Single Responsibility Principle**, which is one of the core concepts of SOLID principles, it is
  often
  good practice to **assign only closely related functionalities to a single class or service**.
  Therefore, functionalities related to Character such as adding a Skill to a Character or getting Skills of a Character
  are generally better placed in CharacterService. It also makes more intuitive sense as you are performing operations
  on
  a Character object.
- Similarly, functions that directly affect Skill objects like creating, retrieving, updating, and deleting a Skill
  should
  be part of SkillService. This makes the design more modular and classes/services more independent, which ultimately
  leads to a more maintainable and understandable codebase.

The key is to ensure that the design choice you make supports good software principles such as high-cohesion,
loose-coupling and modularity.

## Should the `CharacterService` talk to `SkillService`?

It is more efficient to have `CharacterService` interact directly with the `SkillRepository` (or whatever
method you're using to persist and retrieve Skill entities) rather than `SkillService`.

- A possible scenario could be when you are adding a Skill to a Character. The CharacterService would need to ensure
  that the Skill actually exists before it can be added to a Character. In such cases, CharacterService would interact
  with
  the SkillRepository to query the existence of the Skill.
- This setup can help **avoid an unnecessary level of indirection that would result if CharacterService communicates
  with SkillService**.

<blockquote class="callout">
The advantages of the repo-to-service call over the service-to-service call include a less complex call stack, easier
  debugging, and clearer data flows, which can lead to better performance and maintainability. Remember,
  services-to-services calls can sometimes lead to code that's harder to follow and maintain.
</blockquote>

## What about the Controller?

The operation of adding a skill to a character is primarily about modifying a `Character`. Therefore, it makes sense for
this endpoint to reside in the CharacterController.
The way you structure your endpoints often depends on how you want to express relationships between resources in your
API. In RESTful terms, adding a `Skill` to a `Character` could be represented as a `POST` or `PUT` request to the URI
`/characters/{characterId}/skills`, with routing handled by the `CharacterController`.