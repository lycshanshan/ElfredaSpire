---
name: generate-simple-sts2-card
description: Generate Simple Slay the Spire 2 mod card C# scripts by calling CardGenerator.py, and write outputs into the user-requested directory.
---

# Generate Simple STS2 Card

This skill generates Slay the Spire 2 mod card C# files by calling `CardGenerator.py`.

## Use this skill when

Use this skill when the user asks to:
- generate one or more Slay the Spire 2 mod card `.cs` files
- convert card design requirements into generated C# card scripts
- batch generate multiple cards into a specified output directory

## Script capabilities

`CardGenerator.py` supports the following card fields and generated behaviors.

### Base fields
- `character`
- `card_pool`
- `card_name`
- `cost`
- `card_type`
  - `Skill`
  - `Attack`
  - `Power`
  - `Status`
  - `Curse`
  - `Quest`
- `card_rarity`
  - `Common`
  - `Uncommon`
  - `Rare`
  - `Token`
  - `Basic`
  - `Ancient`
  - `Event`
  - `Curse`
  - `Quest`
  - `Status`
- `target_type`
  - `AllEnemies`
  - `AnyEnemy`
  - `AnyAlly`
  - `AllAllies`
  - `Self`
  - `RandomEnemy`
  - `AnyPlayer`
  - `TargetedNoCreature`
  - `Osty`

### Supported keywords
- `Exhaust`
- `Ethereal`
- `Innate`
- `Unplayable`
- `Retain`
- `Sly`
- `Eternal`

### Supported tags
- `Strike`
- `Defend`
- `Minion`
- `OstyAttack`
- `Shiv`

### Supported dynamic vars
- `DamageVar`
- `BlockVar`
- `HealVar`
- `CardsVar`
- `EnergyVar`
- `PowerVar_<PowerName>_`

### Supported upgrades
- `Damage`
- `Block`
- `Heal`
- `Cards`
- `EnergyCost`
- `PowerVar_<PowerName>_`

## Required behavior rules

You must follow all of these rules:

1. If any requested card design contains parts that `CardGenerator.py` cannot implement, generate only the code for the implementable portion.
2. All unimplementable requested features must be written as a comment at the very last line of the generated `.cs` file, unless the user explicitly forbids editing the generated cards afterwards.
3. Card names must use English only.
4. If the user requests batch generation, also create a `Checklist.md` file in the target output directory, unless the user specifies a different checklist location or format (see "User-requested checklist").
5. In `Checklist.md`, use each card's `card_name` as the todo item text.
6. If a card is fully implementable by the script, mark it as checked.
7. If a card is not fully implementable, leave it unchecked.
8. Every requested card must produce exactly one generated `.cs` file, even when nothing but an empty skeleton (base fields plus keywords/tags) is generatable.

## Important implementation notes

### Card name restriction
- `card_name` must be English only.
- Remove spaces if needed.
- Do not use Chinese, Japanese, or other non-English identifiers in the generated C# class name.

### Power naming restriction
For `PowerVar_<PowerName>_`, the `<PowerName>` must be an existing game power type and should normally include the `Power` suffix.
Examples:
- `PowerVar_StrengthPower_`
- `PowerVar_WeakPower_`

### Generated OnPlay semantics
The generated `OnPlay` applies every `PowerVar` power to the card owner's creature regardless of `target_type`. Treat the generated code as a skeleton rather than final semantics, and do not hand-edit it unless the user allows post-generation changes.

### Output location
Generate the `.cs` files into the directory requested by the user. The output file is always named `{card_name}.cs`; to name files by card ID (`{ID}.cs`), pass the ID as `--card-name`.

## What the script can directly express

The script can directly generate:
- simple attack cards with damage
- simple block cards
- healing cards
- draw cards
- gain energy cards
- self-applied power cards
- standard keyword declarations
- standard tag declarations
- standard numeric upgrades
- cost upgrades

## What the script cannot directly express

Assume the following are not directly supported unless the request can be reduced to supported vars, keywords, tags, or upgrades:

- custom conditional logic
- “if” / “when” / “unless” logic
- random card generation
- arbitrary card creation into hand/discard/draw pile
- discard effects
- exhaust pile manipulation beyond keyword declaration
- custom retain behavior logic
- end-of-turn, start-of-turn, on-draw, on-exhaust, on-kill triggers
- multiple separate hit sequences (for “X伤害N次”, skip the damage entirely — do not fall back to a single-hit `DamageVar`)
- special stack consumption like “consume up to X stacks” (消耗至多X层)
- target-state-dependent branching
- status checks like “if target is Weak/Vulnerable then ...”
- hand/deck/discard inspection logic
- summon logic
- minion behavior logic
- custom localization text
- custom VFX/SFX
- custom event hooks
- custom multi-stage resolution
- X-cost special logic
- nonstandard targeting logic beyond provided `target_type`
- effects on arbitrary entities not represented by supported generated behavior

If the user requests unsupported behavior, keep only the supported subset and append one final-line comment describing the unsupported parts.

## Natural language mapping guide

Map user requirements into script arguments using these rules whenever possible.

### Common effect mapping
- “造成X伤害” -> `DamageVar=X`
- “获得X格挡” -> `BlockVar=X`
- “回复X生命” -> `HealVar=X`
- “抽X张牌” -> `CardsVar=X`
- “获得X点能量” -> `EnergyVar=X`
- “获得X层某Power” -> `PowerVar_<PowerName>_=X`

### Upgrade mapping
- “升级后伤害+X” -> `Damage=X`
- “升级后格挡+X” -> `Block=X`
- “升级后治疗+X” -> `Heal=X`
- “升级后抽牌+X” -> `Cards=X`
- “升级后减费1” -> `EnergyCost=-1`
- “升级后某Power层数+X” -> `PowerVar_<PowerName>_=X`

### "x/y" convention (x = pre-upgrade, y = post-upgrade)

When a value is written as `x/y`, express it as base `x` plus an upgrade delta of `y - x`:

- “造成6/9点伤害” -> `DamageVar=6`, `Damage=3`
- “获得5/7点格挡” -> `BlockVar=5`, `Block=2`
- “施加2/3层某Power” -> `PowerVar_<PowerName>_=2`, upgrade `PowerVar_<PowerName>_=1`
- “费用2/1” -> `EnergyCost=-1`
- “消耗2/1层某Power” -> `PowerVar_<PowerName>_=-2`, upgrade `PowerVar_<PowerName>_=1` (net: -1)
- “X费” (X-cost) -> `cost=-1`

### Power consumption / stack loss mapping

Plain consumption and stack loss are expressed with negative `PowerVar` values:

- “消耗X层某Power”（无特殊条件）-> `PowerVar_<PowerName>_=-X`
- “失去X点力量” -> `PowerVar_StrengthPower_=-X`

Not supported — ignore the whole effect:

- “消耗至多X层某Power”（至多 = special consumption type, no negative PowerVar fallback）

### Keyword mapping
- 消耗 -> `Exhaust`
- 虚无 -> `Ethereal`
- 固有 -> `Innate`
- 无法打出 -> `Unplayable`
- 保留 -> `Retain`
- 狡诈 -> `Sly`
- 永恒 -> `Eternal`

### Tag mapping
- 打击 -> `Strike`
- 防御 -> `Defend`
- 随从 -> `Minion`
- Osty攻击 -> `OstyAttack`
- 小刀 -> `Shiv`

Apply the `Strike` tag only to the character's 打击 card and `Defend` only to 防御, unless the design explicitly says otherwise.

### Type / rarity / target inference

- Card type: 攻击 -> `Attack`, 技能 -> `Skill`, 能力 -> `Power`
- Rarity by design section: 初始 -> `Basic`, 先古 -> `Ancient`, 普通 -> `Common`, 罕见 -> `Uncommon`, 稀有 -> `Rare`
- Target: 对所有敌人 -> `AllEnemies`; single target / 对目标 -> `AnyEnemy`; self-only effects -> `Self`; Power cards -> `Self`; 随机 -> `RandomEnemy`

## Workflow

When using this skill, follow this process:

1. Read the user's card requirement carefully.
2. Determine whether the request is for a single card or batch generation.
3. Extract or infer the required base fields:
   - `character`
   - `card_pool`
   - `card_name`
   - `cost`
   - `card_type`
   - `card_rarity`
   - `target_type`
4. Normalize `card_name` to English-only.
5. Map supported requirements into:
   - keywords
   - tags
   - vars
   - upgrades
6. Identify any unsupported requested features.
7. Call `CardGenerator.py` with the proper CLI arguments and output directory.
8. If unsupported features exist, append a single final-line comment to the generated `.cs` file.
9. If this is batch generation, create `Checklist.md` after generating all card files.

## CLI command pattern

Use this pattern:

python {skill_path}/CardGenerator.py \
  --character <character> \
  --card-pool <card_pool> \
  --card-name <card_name> \
  --cost <cost> \
  --card-type <card_type> \
  --card-rarity <card_rarity> \
  --target-type <target_type> \
  [--keywords <kw1> <kw2> ...] \
  [--tags <tag1> <tag2> ...] \
  [--var Key=Value ...] \
  [--upgrade Key=Value ...] \
  --output-dir <target_dir>

## Post-generation unsupported comment format

If unsupported requested features exist, append exactly one final line comment like this:

// Unsupported requested features: <feature1>; <feature2>; <feature3>.

If the design is fully supported, do not append this comment.

## Checklist.md format

For batch generation, create `Checklist.md` in the output directory with this format:

- [x] CardNameA
- [ ] CardNameB
- [x] CardNameC

Rules:
- use `card_name` exactly as the todo item text
- checked means fully supported by `CardGenerator.py`
- unchecked means partially supported or contains unsupported requested features

## User-requested checklist (e.g. `Cards.md`)

When the user specifies a checklist location and format (e.g. a `Cards.md` in a `CheckList/` directory), it overrides the built-in `Checklist.md` behavior. Typical process:

1. Generate every card with the CLI only, then verify the generated file count equals the total card count.
2. Write one row per card, in the exact categories and order of the user's card requirement document:
   - `- [x] {ID} {卡牌名}` — fully generated: every requested effect was mapped to supported parameters
   - `- [ ] {ID} {卡牌名}` — partially generated (some special effects ignored) or an empty skeleton
3. Every card gets exactly one row, including empty skeletons (those stay unchecked).
4. Verify the checklist row count equals the total card count.

Checked status rule: `[x]` only when nothing requested was left out. Any ignored special effect, special consumption type, trigger, or multi-hit keeps the row unchecked.

## Behavior when information is missing

If the user does not provide enough information to safely generate the card, ask for clarification instead of inventing important card metadata.

You may infer only when it is obvious and low-risk, for example:
- an attack card that clearly targets a single enemy -> `target_type=AnyEnemy`
- a skill card that only gives self block/draw/heal -> `target_type=Self`

Do not invent:
- character
- card_pool
- card_rarity
- power type names
unless the user has already provided enough context to make them clear.

## Decision examples

### Example 1: fully supported
User request:
- 1费攻击，对单体造成8伤害，升级+3伤害

Supported mapping:
- `cost=1`
- `card_type=Attack`
- `target_type=AnyEnemy`
- `DamageVar=8`
- `Damage=3`

Result:
- generate the `.cs` file
- no unsupported final-line comment
- mark checked if in batch mode

### Example 2: partially supported
User request:
- 1费攻击，造成8伤害，若目标有易伤则再打一次，升级+3伤害

Supported mapping:
- `cost=1`
- `card_type=Attack`
- `target_type=AnyEnemy`
- `DamageVar=8`
- `Damage=3`

Unsupported:
- “若目标有易伤则再打一次”

Result:
- generate only the supported base attack card
- append final-line unsupported comment
- leave unchecked in `Checklist.md` if in batch mode

### Example 3: batch generation
If the user asks for multiple cards:
1. generate each supported `.cs` file into the requested directory
2. append unsupported comment to each partially supported card file if needed
3. write `Checklist.md` into the same directory

## Execution priority

Always prioritize:
1. generating valid `.cs` files for the implementable subset
2. writing unsupported requested features only as the final-line comment
3. ensuring `card_name` is English-only
4. creating `Checklist.md` for batch generation