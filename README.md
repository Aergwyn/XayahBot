# About
`Xayah Bot` is a Discord Bot primarily intended to use for giving League of Legends statistics (general data about champs, winrates, banrates, etc.). Additionally there are other useful commands which you can see later on.

This project is comepletey written in C# and references the following frameworks:
- Microsoft Entity Framework Core
- Discord.Net - [Link](https://github.com/RogueException/Discord.Net)

To present data `Xayah Bot` will access the following APIs:
- Riot Games API
- Champion.GG API

# Commands
Commands can be triggered by mentioning `Xayah Bot` before using the command syntax except direct messages which don't need that.  

Here are some additional points about how `Xayah Bot` works:
- The title and description of commands of this documentation will differ to the ones provided in the help of `Xayah Bot`. Responses in Discord are meant to be "in character" and thus differently phrased.
- If a command is replied to per direct message (to not clutter chats) the triggering message will receive a reaction to imply it was handled. This prevents users from assuming the Bot stopped working because they can't see a response. If the reaction is missing and the user did not receive a response please panic and contact me.

- [ ] TODO: Champion.GG commands

## Help
The help displays a message with a short overview of existing categories, how to access them and contact data.

**Usage:**  
The keyword to this command is `help` followed by an optional page number.

**Example:**  
![Help Request](XayahBot/docs/pics/helprequest.png?raw=true)  
![Help Response](XayahBot/docs/pics/helpresponse.png?raw=true)  

## "8ball"
The 8ball command answers the triggering post with a random response which can be positive, neutral or negative. Additionally `Xayah Bot` demands actual questions and has witty comments if this requirement is not fulfilled.

**Usage:**  
The keywords to this command are `are`, `is` or `am` followed by a sentence.

**Example:**  
!["8ball"](XayahBot/docs/pics/8ball.png?raw=true)  

## Remind me
Reminders are exactly what the name implies. If the user creates a reminder and it expires he gets notified with the provided message.  
There is a cap how long `Xayah Bot` is allowed to wait and how long the message can be. This is configurable but defaults to 30 days and 100 characters, respectively.

**Usage:**  
This command is split in three parts:
- `remind me [number] [time-unit] [text]` creates a new reminder
- `remind me list` shows a list of active reminder
- `remind me clear` clears the list of active reminder

To clarify the parameter when creating a reminder:
- `[number]` defines how long `Xayah Bot` has to wait before the reminder expires
- `[time-unit]` can be `days, day, d, hours, hour, h, minutes, minute, mins, min, m`
- `[text]` that will be posted once the reminder expires

**Examples:**  
*Creating a reminder*  
![Create Reminder](XayahBot/docs/pics/remindcreate.png?raw=true)  
*Requesting a reminder list*  
![Remind List Request](XayahBot/docs/pics/remindlistrequest.png?raw=true)  
![Remind List Response](XayahBot/docs/pics/remindlistresponse.png?raw=true)  
*Clearing the list*  
![Remind Clear Request](XayahBot/docs/pics/remindclearrequest.png?raw=true)  
![Remind Clear Response](XayahBot/docs/pics/remindclearresponse.png?raw=true)  

## Incidents
Incident notifications are supposed to be a way to get notified if there are any problems with one of League of Legends' services. Information like that is available in different public channels (like [this site](http://status.leagueoflegends.com/#na) for NA) but having a post right in Discord could help getting knowledge so much sooner.  

If there are any subscribers `Xayah Bot` will check regularly (Status-Endpoint in API) if an incident is active and post them in the configured channel. This interval is configurable and defaults to 15 minutes.

**Accessibility:**  
This function is not functional in direct messages or groups. To configure incident notifications the user has to have the `administrator` role for the server.

**Usage:**  
This command is split in three parts:
- `incidents on` followed by a mentioned channel as a post location to activate notifications
- `incidents off` to disable notifications
- `incidents status` shows the current server configuration

**Examples:**  
*Enabling incident notifications*  
![Enabling Notifications](XayahBot/docs/pics/incidentson.png?raw=true)  
*Incident example*  
*TODO (sorry, I couldn't catch one so far)*  
*Requesting status*  
![Notification Status](XayahBot/docs/pics/incidentsstatus.png?raw=true)  
*Disabling incident notifications*  
![Disabling Notifications](XayahBot/docs/pics/incidentsoff.png?raw=true)  

## Champ
The champ command gathers data about a specified champion (stats + stat-growth, spells, skins and misc). This data is only refreshed once on the first request each day (StaticData-Endpoint in API).  

**Usage:**  
They keyword to this command is `champ` followed by the name to search for.  
The name doesn't need to be exact! Special characters and/or whitespace can be ignored and even partial names will work. Though if the given name is too vague and could match multiple champs a different response will appear which lists all matching champions.

**Example:**  
*Requesting with a complete name*  
![Champ Complete Name Request](XayahBot/docs/pics/champrequest.png?raw=true)  
![Champ Complete Name Response](XayahBot/docs/pics/champresponse.png?raw=true)  
*Requesting with a partial name that doesn't match a specific champion*  
![Champ Partial Name Request](XayahBot/docs/pics/champpartialrequest.png?raw=true)  
![Champ Partial Name Response](XayahBot/docs/pics/champpartialresponse.png?raw=true)  

# Contact
If you still have questions, problems or even suggestions you can e-mail me at `aergwyn@t-online.com` or add me in Discord `Aergwyn#8786`.
There is also a [discord server](https://discord.gg/YhQYAFW) where you can reach me, try commands out or invite `Xayah Bot` to your server.

# Legal Information
`Xayah Bot` isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing League of Legends. League of Legends and Riot Games are trademarks or registered trademarks of Riot Games, Inc. League of Legends © Riot Games, Inc.
