# Xayah Bot
Xayah Bot is a Discord Bot intended to use for either fun interactions, some management on roles and access to the Riot Games API. A detailed list of commands will follow later on.

This project is comepletey written in C# and references the following frameworks:
- Microsoft Entity Framework Core
- Discord.Net - [Link](https://github.com/RogueException/Discord.Net)

I try to write as much as possible myself to learn and try different approaches. I welcome any suggestions and feedback (see [Contact](https://github.com/Aergwyn/XayahBot/blob/master/README.md#contact)).

## Features
As already mentioned there are essentially three categories of commands.
1. Fun commands: Something like the 8-ball command ~~or a quiz~~.
2. Informational commands: Retrieving data about a champion or notifying about incidents currently ongoing all fueled by Riot Games API
3. Organisational commands: Excluding users/channels from certain commands to reduce spam or letting XayahBot track your Rank in League and assign corresponding roles

## Usage
Each command is able to be used by prefixing a message with a `.` and then using command syntax. It is also possible to mention Xayah Bot instead.  
Examples:  
- .help
- @Xayah Bot help

Following is a list of all commands. If I learn something new I may rework them to be easier to use or more efficient so they are all subject to change. I'll try to keep this list as up to date as possible.

### Help
**Usable by:** Everyone  
**Usable in:** Everywhere  

**Description:** This help shows you which commands exist and how to use them. As there are just too many commands to list them flat out they are categorized and you will have to follow up that command with the desired category. If the specified category doesn't exist it will default to `help`.  

**Examples:**  
- .help  
Shows the help  
- .help data  
Show the help for the category data

### Data
**Usable by:** Everyone  
**Usable in:** Everywhere  

**Description:** This command shows you Riot API data (Endpoint: `Static-Data-V3`) to a specified champion. Currently this is split between `misc`, `spells` and `stats`.  
The name can be a partial input. If you want to retrieve data from `vikt` you will get a response with data for Viktor. If you try to retrieve data from `a` you will get a list of all champions that have an `a` in their name and should try again.  

**Examples:**  
- .data champ misc viktor  
Shows general data about the specified champion (Title, Tags, Resource and Skins)  
- .data champ stats viktor  
Shows champion stats and statgrowth of the specified champion.  
- .data champ spells viktor  
Shows the spells of specified champion with name, cost, range, cooldown and scaling.

### Remind me
**Usable by:** Everyone  
**Usable in:** Everywhere  

**Description:** This command will remind you after a set amount of time with the specified text when the command was used.  

**Examples:**  
TODO  
- [ ] make command great again

### Incidents
**Usable by:** ?  
**Usable in:** Guild  

**Description:** This command allows enabling/disabling incident tracking from Riot API (Endpoint: `Status-V3`, Regions: EUW, NA, EUNE) to be updated as fast as possible. If an incident gets updates or is resolved `Xayah Bot` will remove old messages to not litter the channel.  

**Examples:**  
- .incidents status  
Shows if incident tracking is currently disabled or not and in which channel updates will be posted  
- .incidents enable #news-channel  
Enables incident tracking and will post updates in the specified channel  
- .incidents disable  
Disables incident tracking


- [ ] Complete list of commands with descriptions and pictures
- [ ] Rework who can use which command by a specific role (if needed) relevant for that command

## Contact
If you got any problems, questions and/or suggestions you can e-mail me at `aergwyn@t-online.com` or add me in Discord (`Aergwyn#8786`).
There is also a [discord server](https://discord.gg/YhQYAFW) where you can reach me, try commands out or invite Xayah Bot to your server.

## Legal Information
`Xayah Bot` isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing League of Legends. League of Legends and Riot Games are trademarks or registered trademarks of Riot Games, Inc. League of Legends © Riot Games, Inc.
