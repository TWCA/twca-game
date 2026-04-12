/* 
-Index for coders-:

intro_scene: Bedroom scene: Robin & Sam
motherintro: Kitchen text between Robin & Mom
feedsam: Trigger once sam has been fed
dogwalk: Outside house
first_notif: 

-Dialog Sections-:
dialog_1: Francis & Robin
dialog_2: Lorenzo & Robin
dialog_3: Selects one of two options
-> group_dialog: Francis and Lorenzo ask about Robin
-> police_dialog: Police have started a search for Robin, due to no contact

meet_sam: Plays when finding Sam
walk_home: Dialog between Sam and Robin on their walk home

end_scene: Dialog between Robin and Mom
credits: Names and roles played during developement
after_credits: Dialog to play during/after credits


random_notif: loads a random notification (Readit, Instancegram or News)
*/

//Variables for controlling story flow
VAR dialog1chat = false
VAR dialog2chat = false
VAR badDay = false
VAR partyPlanned = false
VAR dogmentioned = false



// Intro Start
== intro_scene ==
Bark! #Sam #Voice:VA/SamBarks/Bark1 #disableBehaviours
...Just a few more minutes Sam... #Robin #Voice:VA/IntroScene/JustAFewMinutes
Ruff, #Sam #Voice:VA/SamBarks/Bark4
Ruff! #Voice:VA/SamBarks/Bark3
Okay, okay, I'll get up! #Robin #Voice:VA/IntroScene/IllGetUp #waitForTrigger #enableBehaviours

Alright then, how's my favourite noisy dog this morning? #Robin #Voice:VA/IntroScene/HowMyFavourite #Delay:0.3
Woof! #Sam #Voice:VA/SamBarks/Bark2
Alright, let's get you your breakfast! #Robin #Voice:VA/IntroScene/LetsGetYouBreakfast
Woof, woof! #Sam #Voice:VA/SamBarks/Bark5
#Voice:VA/SamBarks/Bark6
-> END

== motherintro ==
If you're up Robin, I left you some toast on the table. #Mom #Voice:VA/IntroScene/IfYou'reUp
 * [...] #Robin
 * Thanks #Robin #Voice:VA/IntroScene/Thanks
 - I've got a lot of work so I'll be out late tonight, either make yourself something or order in some takeout. #Mom #Voice:VA/IntroScene/OutLateTonight
 Can you also take your dog out? He's been pestering me all morning and I'm very busy. #Mom #Voice:VA/IntroScene/TakeDogOut
 I will #Robin #Voice:VA/IntroScene/IWill
 Enjoy your lazing around today! #Mom #Voice:VA/IntroScene/EnjoyYourLazingAround
 Alright let's get you your breakfast. #Robin #Voice:VA/IntroScene/LetsGetYouBreakfast2 #closePhone #enableBehaviours
 Arf! #Sam #Voice:VA/SamBarks/Bark7
-> END
 
 == feedsam ==
 \*nom nom\* #Sam
 Alright eat up, and let's go for that walk. #Robin #Voice:VA/IntroScene/EatUp
 -> END
 
 == dogwalk ==
 Bark! #Sam #Voice:VA/SamBarks/Bark4 #waitForTrigger
 
 Just a short walk today, okay Sam? #Robin #Voice:VA/IntroScene/JustAShortWalk
 Woof! #Sam #Voice:VA/SamBarks/Bark5
 -> END
 
== first_notif ==
 Hold on one second Sam, I have to check this notification. #Robin #Voice:VA/IntroScene/HoldOnSam #disableBehaviours
 Dangers of social media, with screen time awareness on the decrease! #Notification:News #openPhone #Voice:VA/IntroScene/DangersOfSocialMedia //It's a little on the nose, but I prefer it being like this than the stuck semi
 -> END
 
== after_first_notif ==
 Bit of a scary topic on the news today... #Robin #Voice:VA/IntroScene/BitOfAScaryTopic #disableBehaviours
 Maybe that article was right, I didn't realize I was out here that long. #Robin #Voice:VA/IntroScene/MaybeThatArticleWasRight
 I'm sure that was a long enough walk for you Sam! #Robin #Voice:VA/IntroScene/I'mSureThatWasEnough
 ... #Robin #Delay:0.3
 Sam? #Robin #Voice:VA/IntroScene/Sam #enableBehaviours
 -> END
// Intro End


//Dialogs Start

// Dialog 1 Start
== dialog_1 ==
Hi Robin, just checking in on you, it's been a while since we last talked! #Francis #Voice:VA/InterLevel/HiJustChecking

 * I've had better days #Robin #Voice:VA/InterLevel/I'veHadBetterDays
   ~ badDay = true
   Oh? What's up? #Francis #Voice:VA/InterLevel/OhWhat'sUp
   * * Sam's gone missing, and I haven't found him yet #Robin #Voice:VA/InterLevel/Sam'sGoneMissing
       ~ dogmentioned = true
       Sorry to hear that Robin! Is there anything I can do? #Francis #Voice:VA/InterLevel/SorryToHear
       No, I'm sure that I'll find him soon, I'm okay #Robin #Voice:VA/InterLevel/IllFindHim
       Alright then, good luck then! :D #Francis #Voice:VA/InterLevel/GoodLuck
       Oh and, #Francis #Voice:VA/InterLevel/OkIKnow #IgnoreNextVoice -> checkup_end_pos 
   * * It's just been a bit busy lately #Robin #Voice:VA/InterLevel/ItsBeenBusy
       Fair enough. Anyways, just wanted to check in, make sure you're doing well! #Francis #Voice:VA/InterLevel/FairEnough
       -> checkup_end_pos
 * I've been good #Robin #Voice:VA/InterLevel/IveBeenGood
   That's so great to hear! #Francis #Voice:VA/InterLevel/GreatToHear
   So, #Francis #Voice:VA/InterLevel/SoIKnow #IgnoreNextVoice -> checkup_end_pos
 * [...] #Robin
 -> checkup_end_neg 

== checkup_end_pos ==
~ dialog1chat = true
I know you've been busy the last few times I tried to invite you out, but we'll hang out this time, okay? #Francis #Voice:VA/InterLevel/IKnow
 * Okay #Robin #Voice:VA/InterLevel/Okay
    Sounds like a plan then! #Francis #Voice:VA/InterLevel/SoundsLikeAPlan
    -> END
 * [...] #Robin
    {badDay} Today might not be great, but we'll have a good time soon! #Francis #Voice:VA/InterLevel/We'llFindAGoodTimeSoon
    -> END
== checkup_end_neg ==
~ dialog1chat = false
I know you've been busy the last few times I tried to invite you out, but we'll hang out this time, okay? #Francis #Voice:VA/InterLevel/IKnow
 * Okay #Robin #Voice:VA/InterLevel/Okay
   I know you don't want to talk now, but I'll be there when you do! #Francis #Voice:VA/InterLevel/ThereWhenYouDo
    -> END
 * [...] #Robin
   I'm worried about you Robin. #Francis #Voice:VA/InterLevel/ImWorriedAboutYou
    -> END
//Dialog 1 End


//Dialog 2 Start
== dialog_2 ==
Hey Robin, you weren't in class the other day, and I've just been wondering how you're doing? #Lorenzo #Voice:VA/SceneWLorenzo/WonderingHowYoureDoing
 * I'm not feeling great :( #Robin #Voice:VA/SceneWLorenzo/NotFeelingGood
   So that's why I haven't seen you around! #Lorenzo
   Make sure you rest up and feel better! #Lorenzo #Voice:VA/SceneWLorenzo/SoThatsWhyFeelBetter
    -> class_end_pos
 * Just dealing with something #Robin #Voice:VA/SceneWLorenzo/DealingWithSomething
   ~ badDay = true
   Something to do with your mother? #Lorenzo #Voice:VA/SceneWLorenzo/SomethingToDoWith
   * * No it's Sam. He's gone off on his own #Robin #Voice:VA/SceneWLorenzo/GoneOffOnHisOwn
       That rebelious dog!!! >:( #Lorenzo
       I'm sure you'll find him! #Lorenzo #Voice:VA/SceneWLorenzo/ThatRebeliousDogImSureYoullFindHim
       I hope so too #Robin #Voice:VA/SceneWLorenzo/IHopeSo
       ~ dogmentioned = true
      -> class_end_pos
   * * I don't really want to say #Robin #Voice:VA/SceneWLorenzo/IDontWantToSay
       All good Robin, I'm here if you ever want to talk about it #Lorenzo #Voice:VA/SceneWLorenzo/ImHereIfYouEverWant
      -> class_end_pos
 * [...]
    -> class_end_neg
   
== class_end_pos ==
   ~ dialog2chat = true
   Don't worry, I'll make sure my notes are nice and neat so you can borrow them and catch up! #Lorenzo #Voice:VA/SceneWLorenzo/NiceAndNeat
   Thanks Lorenzo #Robin #Voice:VA/SceneWLorenzo/ThanksLorenzo
   Of course! #Lorenzo #Voice:VA/SceneWLorenzo/OfCourse
    -> END
    
== class_end_neg ==
   ~ dialog2chat = false
   Whatever the reason, we'll be waiting for you Robin. #Lorenzo
   Just.. keep us in the loop, kay? #Lorenzo #Voice:VA/SceneWLorenzo/KeepUsInTheLoop
    -> END
//Dialog 2 End


//Select Dialog 3
== dialog_3 ==
{dialog1chat or dialog2chat: ->group_dialog}
->police_dialog


//Group Dialog Start
== group_dialog ==
Hey Robin, we were talking about having a party soon #Francis #Voice:VA/GroupScene/PartySoon
{!badDay: We know you've been away for a little while, but we'd like to see you again #Lorenzo #Voice:VA/GroupScene/WedReallyLikeToSeeYouAgain}
{badDay: We know it's been a rough time for you, so we'd like to raise your spirits a bit! :D #Lorenzo #VA/GroupScene/RaiseYourSpirits}
{dogmentioned: {dialog1chat: If you found Sam, bring him to the party! #Francis #Voice:VA/GroupScene/BringHimToTheParty}}
{dogmentioned: {!dialog1chat: Bring Sam too, once you find him! #Lorenzo #Voice:VA/GroupScene/BringSamToo}}

* {dogmentioned} I still haven't found him #Robin #Voice:VA/GroupScene/StillHaventFoundHim
    Tell us where you are Robin, we can help #Francis #Voice:VA/GroupScene/TellUsWhereYouAre
    Yeah, we'll make sure you and Sam get home safe! #Lorenzo #Voice:VA/GroupScene/WellMakeSure
    We can't let him stay out for days on end! #Francis #VA/GroupScene/WeCan'tLetHimStayOut
    Days on end? #Robin #VA/GroupScene/DaysOnEnd
    It's been a few days now Robin..? #Lorenzo #VA/GroupScene/It'sBeenAFewDaysNowRobin
    -> home_soon
* {!dogmentioned} I've been trying to find Sam, he's missing :( #Robin #Voice:VA/GroupScene/TryingToFindSam
    Sorry to hear that! #Lorenzo #Voice:VA/GroupScene/SorryToHearThat
    How long has Sam been missing? #Francis #Voice:VA/GroupScene/HowLongHasSamBeenMissing
    He ran off on me a little while ago #Robin #Voice:VA/GroupScene/RanOffOnMe
    You've been out for more than a little while Robin, is Sam the only thing bothering you? #Lorenzo #Voice:VA/GroupScene/MoreThanAFewDays
    Yeah, it's closer to a few days now, so I'm a little worried about you myself #Francis #Voice:VA/GroupScene/CloserToAFewDays
    ->home_soon
* I just need a little bit longer to myself #Robin #Voice:VA/GroupScene/ALittleLongerToMyself
    Robin.. we're worried about you. #Francis #Voice:VA/GroupScene/WereWorriedAboutYou
    We haven't seen you in a few days now... #Lorenzo #Voice:VA/GroupScene/HaventSeenYouInAFewDays
    I know, I'm sorry guys, but I'll come back soon. #Robin #Voice:VA/GroupScene/IllComeBackSoon
    We trust you Robin but please, come back, safe and sound? #Lorenzo #Voice:VA/GroupScene/WeTrustYouRobin
    I will. #Robin #Voice:VA/GroupScene/IWill
    ->END

== home_soon ==
    ~ partyPlanned = true
    Something's felt a bit off in this forest, kinda like I'm time travelling? #Robin #Voice:VA/GroupScene/SomethingsFeltOff
    Ok, weird #Lorenzo #Voice:VA/GroupScene/OkWeird
    I think I hear him just up ahead, I don't think I'll be much longer. #Robin  #Voice:VA/GroupScene/HearHimJustUpAhead
    Alright, we'll wait, but just come back soon. #Francis #Voice:VA/GroupScene/ComeBackSoon
    Don't worry, I'll be home soon. #Robin #Voice:Voice:VA/GroupScene/DontWorry
    We'll have that party once Sam's found :D #Lorenzo #Voice:VA/GroupScene/We'llHaveThatPartyOnceSam'sFound
    ->END
//Group Dialog End

//Police Start
== police_dialog ==
    This is an Emergency Missing Person Alert #Police #Voice:VA/CopDialog/MissingPerson
    The current whereabouts of Robin Wilf is unknown, and anyone with information is urged to report it to the nearest authorities. #Police #Voice:VA/CopDialog/CurrentWhereabouts
    Robin's mother noticed that they hadn't been home for a few days and reached out to Robin's friends, who hadn't heard anything from them either. #Police #Voice:VA/CopDialog/RobinsMother
    It is believed that Robin was got lost after taking their dog, Sam for a walk. #Police #Voice:VA/CopDialog/ItIsBelieved
    The authorities are combing the areas near the Wilf residence, but no luck in the search just yet. #Police #Voice:VA/CopDialog/TheAuthorities
    And to Robin, if you're hearing this, please come home, everyone's worried about you. #Police #Voice:VA/CopDialog/PleaseComeHome
-> END
//Police End


//Meet Sam
== meet_sam ==
    Bark! #Sam #Voice:VA/SamBarks/Bark7 #Delay:0.3 #disableSam
    Is that barking? #Robin #Voice:VA/RobinLevelBarks/IsThatBarking #waitForTrigger
    Arf! #Sam #Voice:VA/SamBarks/Bark3 #enableSam
    Sam! There you are! #Robin #Voice:VA/FoundSam/SamThereYouAre
    Woof! #Sam #Voice:VA/SamBarks/Bark5
    I've been all over looking for you. #Robin #Voice:VA/FoundSam/AllOverLookingForYou
    I'm sorry I lost track of time and left you on your own. #Robin #Voice:VA/FoundSam/LeftYouOnYourOwn
    But we're together again now, so let's head home. #Robin #Voice:VA/FoundSam/TogetherAgain
    Arf! #Sam #Voice:VA/SamBarks/Bark6
    -> END


//Walk
== walk_home ==
    You would not believe what I've been through getting here Sam. #Robin #Voice:VA/FoundSam/WhatIveBeenThrough
    Woof? #Sam #Voice:VA/SamBarks/Bark1 #waitForTrigger
    I started slipping through time and had to figure how to get through the forest. #Robin #Voice:VA/FoundSam/JourneyExplanationPart1
    And I redirected a river with a log, and jumped some pretty large gaps and even managed to put out a fire! #Robin #Voice:VA/FoundSam/JourneyExplanationPart2 #waitForTrigger
    Arf! #Sam #Voice:VA/SamBarks/Bark4
    Yeah, I'm a little tired. #Robin #Voice:VA/FoundSam/ALittleTired #waitForTrigger
    ... #Robin
    
    I just remembered! #Robin #Voice:VA/FoundSam/IJustRemembered
    {partyPlanned: I got invited out to see my friends, Sam! And you're invited too! #Robin #Voice:VA/FoundSam/You'reInvitedTooSam #waitForTrigger}
    {not (dialog1chat or dialog2chat): I think Mom called the police, so I'll probably be in trouble when we get back. #Robin #Voice:VA/FoundSam/IThinkMomCalledThePolice #waitForTrigger}
    {(dialog1chat or dialog2chat) and not partyPlanned: I've been a bit of a bad friend towards the guys, I should send them a text when I get back. #Robin #Voice:VA/FoundSam/I'veBeenABadFriend  #waitForTrigger}

    //Would prefer to have a notification light for this part, but if not remove added line
    ->last_notif

== last_notif ==
    Ding! #notificationSound #Delay:0.5
    Oh! #Robin #Voice:VA/FoundSam/OhNotif #Delay:0.5
    Actually, on second thought, I think I'm good for a little while. #Robin #Voice:VA/FoundSam/OnSecondThought #Delay:1.0 #waitForTrigger
    I'd rather not lose you again, Sam, one time is enough for me. #Robin #Voice:VA/FoundSam/IdRatherNotLoseYouAgain
    Woof! #Sam #Voice:VA/SamBarks/Bark3
    I'm glad you agree let's go home #Robin #Voice:VA/FoundSam/ImGladYouAgree
    Bark, bark! #Sam #Voice:VA/SamBarks/Bark7
    ->END

//Ending Start
== end_scene ==
    Woof! #Sam #Voice:VA/SamBarks/Bark2 #waitForTrigger
    I'm home.. #Robin #Voice:VA/ArriveHome/ImHome #waitForTrigger
    Robin, where were you for so long! Ne... never mind. I'm just happy you're home! #Mom #Voice:VA/ArriveHome/ImHappyYoureHomeAngryVer
    I asked your friends, called the school, no one had seen you. #Mom #Voice:VA/ArriveHome/NoOneHadSeenYouAngryVer
    {not (dialog1chat and dialog2chat): I even called the police I was so worried! #Mom #Voice:VA/ArriveHome/IEvenCalledThePolice}
    I'm sorry Mom, I got lost. #Robin #Voice:VA/ArriveHome/ImSorryMom
    \*sigh* It's okay dear, I'm just glad you came home. #Mom #Voice:VA/ArriveHome/ImjustGladYoureHome
    Arf! #Sam #Voice:VA/SamBarks/Bark2
    Of course you too, Sammy. #Mom #Voice:VA/ArriveHome/YouTooSammy
    Woof! #Sam #Voice:VA/SamBarks/Bark1
    ... #Robin
    I love you, Mom. #Robin #Voice:VA/ArriveHome/ILoveYouMom
    I love you too, sweetie. #Mom #Voice:VA/ArriveHome/ILoveYouTooSweetie
    ... #Delay:3.0
    The End #returnToMainMenu
    ... #Delay:2.0
    -> END
    
// After or during credits I'd put these
== after_credits ==
    //This whole section for credits with Mom & Robin
    {partyPlanned: So I hear you planned a little get together with your friends? #Mom #Voice:VA/ArriveHome/PlannedALittleGetTogether}
    {partyPlanned: Oh yeah, they heard I was having a hard time finding Sam.  #Robin #Voice:VA/ArriveHome/TheyHeardIWasHavingAHardTime}
    {partyPlanned: Glad to see you're socializing a bit! #Mom #Voice:VA/ArriveHome/GladToSeeYou'reSocializing}
    {partyPlanned: Sorry I've been a bit of burden to everyone. #Robin #Voice:VA/ArriveHome/SorryI'veBeenABurden}
    {partyPlanned: Oh shush you, you're still young, not too late for changes in attitude #Mom #Voice:VA/ArriveHome/OhShushYou}
    {not partyPlanned: Your friends were worried about you when I called to ask if they'd seen you. #Mom #Voice:VA/ArriveHome/FriendsWereWorriedAboutYou}
    {not partyPlanned: They mentioned that they were hoping to hangout with you some day soon. #Mom #Voice:VA/ArriveHome/MentionedTheyWereHoping}
    {not partyPlanned: Yeah, I'd like that. I'll message then soon and plan something #Robin #Voice:VA/ArriveHome/I'dLikeThat}
    {not (dialog1chat or dialog2chat): I suppose I'll have to call that police officer back and tell him you returned safe and sound. #Mom #Voice:VA/ArriveHome/I'llHaveToCallThePoliceOfficer}
    ->END

//Dialogs End


== credits ==
 Leigh, Lead Designer and Artist
 Jacob, Game Design and Audio
 Wren, Producer and Programmer and Voice of Francis and Sam
 Adam, Programmer
 Liam, Programmer
 Nishchay, Programmer
 Noel, Writer
 Lily, Executive Producer
 
 ___, voice of Robin
 Wren, Voice of Francis and Sam
 ___, voice of Lorenzo
 ___, voice of Robin's mother
 Jacob, Voice of Sheriff
 ->END



// Notifications Start
== random_notif ==
// Randomizes notifs
{~->reddit1|->reddit2|->reddit3|->reddit4|->reddit5|->reddit6|->reddit7|->reddit8|->insta1|->insta2|->insta3|->insta1|->insta2|->insta3|->insta1|->insta2|->insta3|->insta4|->news1|->news2|->news3|->news4|->news5|->news6|->news7|->news8|->news9|->news10|->news11}

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

== reddit5 ==
Fell through my ceiling. Trying to fix it before the wife wakes up. #Notification:Readit #Voice:VA/Notifications/FellThroughCeiling
//Looks like a bomb went off! 
-> END

== reddit6 ==
I fell asleep with my sister after playing video games and now my wife's pissed, AITA? #Notification:Readit #Voice:VA/Notifications/WifesPissedAITA
//Every once in a while, there's a title that makes you go, huh?
-> END

== reddit7 ==
Fall of Berlin Wall was a result of an "clerical error" by an officer. #Notification:Readit #Voice:VA/Notifications/FallOfBerlinWall
-> thisisneat

== reddit8 ==
Why do the banks keep denying my attempt at mortgage fraud? #Notification:Readit #Voice:VA/Notifications/BanksDeny
//Couldn't tell ya. 
->END

== reddit9 ==
A fan struck in the head by a puck over the glass at Oilers game! #Notification:Readit #Voice:VA/Notifications/StruckAtOilersGame
// Ouch.
->END

== insta1 ==
// Oh, someone sent me something?
#Voice:VA/Notifications/SomeoneSentMeSomething
// I would record this as "User sent you a post.", unless you want all names spoken, in which I'// switch this one
{~@lonelygoose3|@richard.gestral|@hollow_hannah_night|@serena.stardust} sent you a post. #Notification:Instancegram 
->END

== insta2 ==
{~@plextrongames #Voice:VA/Notifications/PlexTron|@yeg.news #Voice:VA/Notifications/YEG|@WorkaholicWisp #Voice:VA/Notifications/WorkaholicWisp|@Velvet.Tides #Voice:VA/Notifications/VelvetTides|@itsmarinalow #Voice:VA/Notifications/ItsMarinaLow} has posted for the first time in a while. #Notification:Instancegram
->END

== insta3 ==
{~@poluxsi|@regularold.ow|@makobarrett|@naytiba_eve} has liked your post. #Notification:Instancegram #Voice:VA/Notifications/SomeoneLikedMyPost
-> END

== insta4 ==
Here's a reel you might like! #Notification:Instancegram #Voice:VA/Notifications/Here'sAReel
->END

== news1 ==
Stuck semi-truck closed High Level bridge. #Notification:News #Voice:VA/Notifications/StuckSemiTruck
// Man, this news clipping seems to be everywhere 
->END

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

== news6 ==
Public alert for measles exposure! #Notification:News #Voice:VA/Notifications/MeaslesExposure
-> ohboy

== news7 ==
Even with new projects on the way, how much space is needed at Edmonton Public Schools? #Notification:News #Voice:VA/Notifications/PublicSchools
//Hopefully enough to get Charles out of my classroom!
-> END

== news8 ==
Inspection launched into Calgary's watermain breaks #Notification:News #Voice:VA/Notifications/InspectionLaunched
-> ohboy

== news9 ==
Arborist rescues cat from 8-storey-tall tree in east Ottawa #Notification:News #Voice:VA/Notifications/RescueCat
-> animalreply

== news10 ==
Banff National Park breaks visitation record - again #Notification:News #Voice:VA/Notifications/Banff
//I miss going out to the mountains.
-> END

== news11 ==
Canada slips further down in World Happiness rankings, due in part to social media use. #Notification:News #Voice:VA/Notifications/CanadaWorldRankings
//Of course, it's always the phone! 
-> END

== news12 ==
Report: Baby born inside Rogers Place during Oilers game #Notification:News #Voice:VA/Notifications/HopefullyHe'sAFan
//Hopefully he's a fan
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



// Barks Start
== bark_feed_sam ==
I should go to the kitchen, I need to feed Sam. #Robin #Voice:VA/RobinLevelBarks/TimeToGoToTheKitchen
-> END

== bark_kibble ==
Sam's kibble is there in the corner. #Robin #Voice:VA/RobinLevelBarks/Kibble
-> END

== bark_bowl ==
Where did I put Sam's bowl again? #Robin #Voice:VA/RobinLevelBarks/Bowl
-> END

== bark_reception ==
I wonder what's happening online. I might get reception somewhere else. #Robin #Voice:VA/RobinLevelBarks/Reception
-> END

== bark_gate ==
I need to get past that gate. #Robin #Voice:VA/RobinLevelBarks/GetPastGate
-> END

== bark_gate_locked ==
Why does the gate have to be locked. #Robin #Voice:VA/RobinLevelBarks/GateLocked
-> END

== bark_fire_spread ==
What if the fire spreads. #Robin #Voice:VA/RobinLevelBarks/FireSpreads
-> END

== bark_avoid_fire ==
I don't want to go near those flames. #Robin #Voice:VA/RobinLevelBarks/DontWannaGoNearFlames
-> END

== bark_uneven_ground ==
The ground isn't flat here, the bucket just falls. #Robin #Voice:VA/RobinLevelBarks/GroundNotFlat
-> END

== bark_slow_fill ==
The bucket is filling... vveeerrryyy slllloooowwwwlllyyy. #Robin #Voice:VA/RobinLevelBarks/CoupleHoursBucket #TODO
-> END

== bark_slow_fill_hours ==
It will take a couple hours for this bucket to fill. #Robin #Voice:VA/RobinLevelBarks/CoupleHoursBucket
-> END

== bark_could_jump ==
I think I could jump over that ledge. #Robin #Voice:VA/RobinLevelBarks/ThinkICouldJumpOverLedge
-> END

== bark_big_jump ==
That's a pretty big jump. #Robin #Voice:VA/RobinLevelBarks/ThatsABigJump
-> END

== bark_jump_momentum ==
I need to get moving faster. #Robin #Voice:VA/RobinLevelBarks/NeedToRunFaster
-> END

== bark_fast_water1 ==
The water is too fast, I don't want to get near it. #Robin #Voice:VA/RobinLevelBarks/WaterTooFastNotNearIt
-> END

== bark_fast_water2 ==
The water is too fast, it will just wash away. #Robin #Voice:VA/RobinLevelBarks/WashAway
-> END

== bark_cant_cross ==
I can't cross the water right now. #Robin #Voice:VA/RobinLevelBarks/CantCrossWaterRn
-> END

== bark_log_blocks_river ==
It looks like that log is blocking the river. #Robin #Voice:VA/RobinLevelBarks/LogBlockingRiver
-> END

== bark_jump_up ==
I think I could jump up that. #Robin #Voice:VA/RobinLevelBarks/JumpUpThat
-> END

== bark_sam_follow ==
I'm sure Sam will follow me. #Robin #Voice:VA/RobinLevelBarks/SamWillFollow
-> END

== bark_get_home ==
I need to get home #Robin #Voice:VA/RobinLevelBarks/NeedToGetHome
-> END

== bark_good_dog ==
Who's a good dog? #Robin #Voice:VA/RobinLevelBarks/WhosAGoodDog
-> END

// Barks End
