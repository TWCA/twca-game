/* 
-Index for coders-:

intro_scene: Intro scene, includes dog food section

-Dialog Sections-:
dialog_1: Friend & Robin (VOICED)

random_notif: loads a random notification (Reddit, Instagram or News)
*/


// Intro Start
== intro_scene ==
Bark! # Sam
...Just a few more minutes Sam... # Robin
Bark, bark! # Sam
Okay, okay, I'll get up! # Robin

Alright then, how's my favourite noisy dog this morning? # Robin
Woof! # Sam
That's good then, shall we go get some breakfast? # Robin
Woof, woof! # Sam
-> motherintro

// Could be converted to a text exchange if preferred
== motherintro ==
Good morning Robin, I left you some toast on the table. # Mother
 * [...] # Robin
 * Thanks. # Robin
 - I've got a lot of work so I'll be out late tonight, either make yourself something or order in a dinner. # Mother
 Oh, and could you take Sam out for a walk, he gets too energetic when he's left inside too long. # Mother
 I will. # Robin
 Alright, see you later then, I'm off. # Mother
 -> feedsam
 
 // Cut this section if we want picking up items introduced later, but I thought this might be an easy way to introduce it.
 == feedsam ==
 Alright let's get you your breakfast. # Robin
 Arf! # Sam
 // Add any needed hint dialogue here
 \*nom nom\* # Sam
 Alright eat up, and let's go for that walk. # Robin
 -> dogwalk
 
 == dogwalk ==
 Just a short walk today, okay Sam? # Robin
 Woof! # Sam
 Hold on one second Sam, I have to check this notification. # Robin
 ...Sam? # Robin
-> END
// Intro End


//Dialogs Start

// Dialog 1 Start
== dialog_1 ==
Hi Robin, just checking in on you, it's been a while since we last talked! #Friend #Voice:VA/InterLevel/HiJustChecking

 * [...] #Robin
 -> checkup_end 
 * I've been okay. #Robin #Voice:VA/InterLevel/IveBeenOkay
   Oh? Something happening? #Friend #Voice:VA/InterLevel/SomethingHappening
   * * Sam's gone missing, and I haven't found him yet. #Robin #Voice:VA/InterLevel/SamsGone
       Sorry to hear that Robin! Anything I can do? #Friend #Voice:VA/InterLevel/SorryToHear
       No, I'm sure I'll find him soon, I'm okay. #Robin #Voice:VA/InterLevel/IllFindHim
       Alright then, good luck! #Friend #Voice:VA/InterLevel/GoodLuck
       Oh and, #Friend #Voice:VA/InterLevel/OkIKnow #IgnoreNextVoice -> checkup_end 
   * * It's just been a bit busy lately. #Robin #Voice:VA/InterLevel/ItsBeenBusy
       Fair enough. Anyways, just wanted to check in, make sure you're doing well! #Friend #Voice:VA/InterLevel/FairEnough
       -> checkup_end
 * I've been good. #Robin #Voice:VA/InterLevel/IveBeenGood
   That's great to hear! #Friend #Voice:VA/InterLevel/GreatToHear
   So, #Friend #Voice:VA/InterLevel/SoIKnow #IgnoreNextVoice -> checkup_end 

== checkup_end ==
I know you've been busy the last few times I tried to invite you out, but we'll hang out this time, okay? #Friend #Voice:VA/InterLevel/IKnow
 * [...] #Robin
    -> END
 * Okay. #Robin #Voice:VA/InterLevel/Okay
    -> END
//Dialog 1 End

//Dialogs End


// Notifications Start
== random_notif ==
// Randomizes notifs
{~->reddit1|->reddit2|->reddit3|->reddit4|->insta1|->insta2|->insta1|->insta2|->news1|->news2|->news3|->news4|->news5}

//  Notifs
== reddit1 ==
My husband hand-delivered our baby boy at Winterburn Costco? #Notification:Readit #Voice:VA/Notifications/CostcoBaby
// Does the baby get a free membership?
->END

== reddit2 ==
I kicked my mother-in-law out while I was on my period, AITA? #Notification:Readit #Voice:VA/Notifications/AITAWithAhole
->ohboy

== reddit3 ==
For anyone wondering how the animals at the Edmonton Valley Zoo were doing in -27° last week… here’s Amba! #Notification:Readit #Voice:VA/Notifications/EdmontonValleyZoo
->animalreply

== reddit4 ==
Fox slept on my outdoor couch. #Notification:Readit #Voice:VA/Notifications/FoxOutdoorCouch
->animalreply

== insta1 ==
// Oh, someone sent me something?
#Voice:VA/Notifications/SomeoneSentMeSomething
// I would record this as "User sent you a post.", unless you want all names spoken, in which I'// switch this one
{~lonelygoose3|richard.gestral|hollow_hannah_night|serena.stardust} sent you a post. #Notification:Instancegram
->END

== insta2 ==
{~plextrongames #Voice:VA/Notifications/PlexTron|yeg.news #Voice:VA/Notifications/YEG|WorkaholicWisp #Voice:VA/Notifications/WorkaholicWisp|Velvet.Tides #Voice:VA/Notifications/VelvetTides|itsmarinalow #Voice:VA/Notifications/ItsMarinaLow} has posted for the first time in a while. #Notification:Instancegram
->END

== news1 ==
Stuck semi-truck closed High Level bridge. #Notification:News #Voice:VA/Notifications/StuckSemiTruck
->ohboy

== news2 ==
50 years of the Muttart Conseratory: More than just pyramids and plants #Notification:News #Voice:VA/Notifications/50Years
->thisisneat

== news3 ==
New Telus World of Science exhibit unveiled! #Notification:News #Voice:VA/Notifications/TelusWorldOfScience
->thisisneat

== news4 ==
Stolen firefighters' 'Jaws of Life' used in multiple break-and-enters #Notification:News #Voice:VA/Notifications/JawsOfLife
->END

== news5 ==
New rollercoaster parts spotted, potentially the new coaster in Galaxyland? #Notification:News #Voice:VA/Notifications/GalaxyLandCoaster
-> END

// Repeatable responses
== ohboy ==
// Oh boy..
#Voice:VA/Notifications/OhBoy
->END

== thisisneat ==
// Hey, this seems interesting.
#Voice:VA/Notifications/ThisSeemsInteresting
->END

== animalreply ==
//That animal is adorable!
#Voice:VA/Notifications/CuteAnimal
->END
// Notifications End
