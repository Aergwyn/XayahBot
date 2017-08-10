# Xayah Bot
`Xayah Bot` is a Discord Bot intended to use for giving League of Legends statistics (general data about champs, winrates, banrates, etc.). Additionally there are other useful commands which you can see later on.

This project is comepletey written in C# and references the following frameworks:
- Microsoft Entity Framework Core
- Discord.Net - [Link](https://github.com/RogueException/Discord.Net)

To present data `Xayah Bot` will access the following APIs:
- Riot Games API
- Champion.GG API

I try to write as much as possible myself to learn and try different approaches. I welcome any feedback (see [Contact](XayahBot#contact)).

# Commands
Commands can be triggered by mentioning `Xayah Bot` first before using the command syntax.  
Exception to this are direct messages where it's not necessary to mention at all.  

Here are some additional points about how `Xayah Bot` works:
- The title and description of commands will differ between here and responses from `Xayah Bot`. Responses in Discord are meant to be "in character".
- If a command is replied per direct message (to not clutter chats) the triggering message will receive a reaction to imply it was handled. This prevents users from assuming the Bot stopped working because they can't see a response. If the reaction is missing and the user did not receive a response please panic and [contact](XayahBot#contact) me.
- `Xayah Bot` has two "modes". If the response is an embed it's "work" and if the response is usual text it's "casual". This is a detail to differentiate between commands with replies that had consequences and those that are just banter (e.g. 8ball).

__Command List__
- Help
- 8ball
- Remind me
- Incidents
- Champ
- [ ] TODO: Champion.GG commands

### Help
**Description:**  
The help displays a message with a short overview what categories exist, how to access them, how to use the help and contact data. The response will be found in a direct message to no clutter chat.

**Usage:**  
The keyword to this command is `help` followed by an optional page number.

**Example:**  
TODO

### 8ball
**Description:**  
The 8ball command answers the triggering post with a random response which can be positive, neutral or negative. Additionally `Xayah Bot` demands actual questions and has responses if this requirement is not fulfilled.

**Usage:**  
The keywords to this command are `are`, `is` or `am` followed by a sentence.

**Example:**  
TODO

### Remind me
**Description:**  
Reminders are exactly what the name implies. If the user creates a reminder and it expires he gets notified with the provided message.  
There is a cap how long `Xayah Bot` is allowed to wait and how long the message can be. This is configurable but per default 30 days and 100 characters, respectively.

**Usage:**  
This command is split in three parts:
- `remind me [number] [time-unit] [text]` -> creates a new reminder
- `remind me list` -> shows a list of active reminder (DM response)
- `remind me clear` -> clears the list of active reminder (DM response)

To clarify the parameter when creating a reminder:
- `[number] [time-unit]` -> defines how long `Xayah Bot` has to wait before the reminder expires
- `[text]` -> text that shall be posted once the reminder expires

**Example:**  
TODO

### Incidents
**Description:**  
Incident notifications are supposed to be a way to get notified if there are any problems with one of League of Legends services. Usually this information is available at [this site](http://status.leagueoflegends.com/#na) (that was an example for NA) or over other public channels.  

If there are any subscribers `Xayah Bot` will check regularly if an incident is active and post them in the configured channel. This interval is configurable and defaults to 15 minutes.

**Accessibility:**  
This function is only available for a server. Direct messages or groups are excluded. To configure incident notifications the user has to have the `administrator` role for the server.

**Usage:**  
This command is split in three parts:
- `incidents on [channel]` -> enables incident notifications and will use the mentioned channel to post
- `incidents off` -> disables incident notifications
- `incidents status` -> shows the current server configuration

**Example:**  
TODO

### Champ
**Description:**  
The champ command gathers data about a specified champion (stats + stat-growth, spells, skins and misc). This data is only refreshed once on the first request each day and the response will be found in a direct message to no clutter chat.  

**Usage:**  
They keyword to this command is `champ` followed by the name to search for.  
The name to search doesn't need to be exact! Special characters and/or whitespace can be ignored and even partial names will work. Though if the given name is too vague and could match multiple champs a different response will appear which lists all matching champions.

**Example:**  
TODO

## Contact
If you still questions, problems or even suggestions you can e-mail me at `aergwyn@t-online.com` or add me in Discord `Aergwyn#8786`.
There is also a [discord server](https://discord.gg/YhQYAFW) where you can reach me, try commands out or invite Xayah Bot to your server.

## Legal Information
`Xayah Bot` isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing League of Legends. League of Legends and Riot Games are trademarks or registered trademarks of Riot Games, Inc. League of Legends © Riot Games, Inc.
