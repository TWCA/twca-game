/* 
-Index for coders-:

intro_scene: Intro scene, includes dog food section (Robin, Sam, Mom)

-Dialog Sections-:
dialog_1: Francis & Robin (Semi-voiced, I added lines after the ends)
dialog_2: Lorenzo & Robin
dialog_3: Selects one of two options
-> group_dialog: Francis and Lorenzo ask about Robin
-> police_dialog: Police have started a search for Robin, due to no contact

meet_sam: Plays when finding Sam
walk_home: Dialog between Sam and Robin on their walk home

end_scene: Dialog between Robin and Mom


random_notif: loads a random notification (Readit, Instancegram or News)
*/

//Variables for controlling story flow
VAR dialog1chat = false
VAR dialog2chat = false
VAR dogmentioned = false


// Intro Start
== intro_scene ==
Bark! #Sam #Voice:VA/SamBarks/Bark1 #disableBehaviours
...Just a few more minutes Sam... #Robin #Voice:VA/IntroScene/JustAFewMinutes
Bark, bark! #Sam #Voice:VA/SamBarks/Bark4
#Voice:VA/SamBarks/Bark3
Okay, okay, I'll get up! #Robin #Voice:VA/IntroScene/IllGetUp #waitForTrigger #enableBehaviours

Alright then, how's my favourite noisy dog this morning? #Robin #Voice:VA/IntroScene/HowMyFavourite #Delay:0.3
Woof! #Sam #Voice:VA/SamBarks/Bark2
That's good then, shall we go get some breakfast? #Robin #Voice:VA/IntroScene/LetsGetYouBreakfast
Woof, woof! #Sam #Voice:VA/SamBarks/Bark5
#Voice:VA/SamBarks/Bark6
-> END

// Could be converted to a text exchange if preferred
== motherintro ==
Good morning Robin, I left you some toast on the table. #Mom #Voice:VA/IntroScene/ToastOnTable
 * [...] #Robin
 * Thanks. #Robin #Voice:VA/IntroScene/Thanks
 - I've got a lot of work so I'll be out late tonight, either make yourself something or order in some take-out. #Mom #Voice:VA/IntroScene/OutLateOrderTakeout
 Oh, and could you take Sam out for a walk, he gets too energetic when he's left inside too long. #Mom
 I will. #Robin #Voice:VA/IntroScene/IWill
 Alright, see you later then, I'm off. #Mom #Voice:VA/IntroScene/ImOff
 Alright let's get you your breakfast. #Robin #Voice:VA/IntroScene/LetsGetYouBreakfast #closePhone #enableBehaviours
 Arf! #Sam #Voice:VA/SamBarks/Bark7
-> END
 
 // Trigger once sam has been feed
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
 Stuck semi-truck closed High Level bridge. #Notification:News #Voice:VA/Notifications/StuckSemiTruck #openPhone
-> END

== after_first_notif ==
 Sam? #Robin #Voice:VA/IntroScene/Sam #enableBehaviours
-> END
// Intro End


//Dialogs Start

// Dialog 1 Start
== dialog_1 ==
Hi Robin, just checking in on you, it's been a while since we last talked! #Francis #Voice:VA/InterLevel/HiJustChecking

 * I've been okay. #Robin #Voice:VA/InterLevel/IveBeenOkay
   Oh? Something happening? #Francis #Voice:VA/InterLevel/SomethingHappening
   * * Sam's gone missing, and I haven't found him yet. #Robin #Voice:VA/InterLevel/SamsGone
       ~ dogmentioned = true
       Sorry to hear that Robin! Anything I can do? #Francis #Voice:VA/InterLevel/SorryToHear
       No, I'm sure I'll find him soon, I'm okay. #Robin #Voice:VA/InterLevel/IllFindHim
       Alright then, good luck! #Francis #Voice:VA/InterLevel/GoodLuck
       Oh and, #Francis #Voice:VA/InterLevel/OkIKnow #IgnoreNextVoice -> checkup_end_pos 
   * * It's just been a bit busy lately. #Robin #Voice:VA/InterLevel/ItsBeenBusy
       Fair enough. Anyways, just wanted to check in, make sure you're doing well! #Francis #Voice:VA/InterLevel/FairEnough
       -> checkup_end_pos
 * I've been good. #Robin #Voice:VA/InterLevel/IveBeenGood
   That's great to hear! #Francis #Voice:VA/InterLevel/GreatToHear
   So, #Francis #Voice:VA/InterLevel/SoIKnow #IgnoreNextVoice -> checkup_end_pos
 * [...] #Robin
 -> checkup_end_neg 

== checkup_end_pos ==
~ dialog1chat = true
I know you've been busy the last few times I tried to invite you out, but we'll hang out this time, okay? #Francis #Voice:VA/InterLevel/IKnow
 * Okay. #Robin #Voice:VA/InterLevel/Okay
    Sounds like a plan then! #Francis #Voice:VA/InterLevel/SoundsLikeAPlan
    -> END
 * [...] #Robin
    -> END

== checkup_end_neg ==
~ dialog1chat = false
I know you've been busy the last few times I tried to invite you out, but we'll hang out this time, okay? #Francis #Voice:VA/InterLevel/IKnow
 * Okay. #Robin #Voice:VA/InterLevel/Okay
   I know you don't want to talk now, but I'll be here when you do. #Francis #Voice:VA/InterLevel/ThereWhenYouDo
    -> END
 * [...] #Robin
   I'm worried about you Robin. #Francis #Voice:VA/InterLevel/ImWorriedAboutYou
    -> END
//Dialog 1 End


//Dialog 2 Start
== dialog_2 ==
Hey Robin, you weren't at class the other day, and I'll just been wondering how you've been? #Lorenzo #Voice:VA/SceneWLorenzo/WonderingHowYoureDoing
 * I'm not feeling great. 🤒 #Robin #Voice:VA/SceneWLorenzo/NotFeelingGood
   So that's why I haven't seen you around! Make sure to rest up and feel better! #Lorenzo #Voice:VA/SceneWLorenzo/SoThatsWhyFeelBetter
    -> class_end_pos
 * Just dealing with something. #Robin #Voice:VA/SceneWLorenzo/DealingWithSomething
   Something to do with your mother? #Lorenzo #Voice:VA/SceneWLorenzo/SomethingToDoWith
   * * No, Sam's just gone off on his own. #Robin #Voice:VA/SceneWLorenzo/GoneOffOnHisOwn
       That rebelious dog!!! I'm sure you'll find him. #Lorenzo #Voice:VA/SceneWLorenzo/ThatRebeliousDogImSureYoullFindHim
       I hope so too. #Robin #Voice:VA/SceneWLorenzo/IHopeSo
       ~ dogmentioned = true
      -> class_end_pos
   * * I'd don't want to say. #Robin #Voice:VA/SceneWLorenzo/IDontWantToSay
       All good Robin, I'm here to listen if you ever want to talk about it. #Lorenzo #Voice:VA/SceneWLorenzo/ImHereIfYouEverWant
      -> class_end_pos
 * [...]
    -> class_end_neg
   
== class_end_pos ==
   ~ dialog2chat = true
   Don't worry, I'll make sure my notes are nice and neat so you can borrow them and catch up! #Lorenzo #Voice:VA/SceneWLorenzo/NiceAndNeat
   Thanks, Lorenzo. #Robin #Voice:VA/SceneWLorenzo/ThanksLorenzo
   Of course! #Lorenzo #Voice:VA/SceneWLorenzo/OfCourse
    -> END
    
== class_end_neg ==
   ~ dialog2chat = false
   Whatever the reason, we'll be waiting for you Robin. Just.. keep us in the loop, okay? #Lorenzo #Voice:VA/SceneWLorenzo/KeepUsInTheLoop
    -> END
//Dialog 2 End


//Select Dialog 3
== dialog_3 ==
{dialog1chat or dialog2chat: ->group_dialog}
->police_dialog


//Group Dialog Start
== group_dialog ==
Hey Robin, we were talking about having a party soon. #Francis #Voice:VA/GroupScene/PartySoon
We know you've been away for a little while, but we'd like to see you again. #Lorenzo #Voice:VA/GroupScene/WedReallyLikeToSeeYouAgain
{dogmentioned: {dialog1chat: If you found Sam, bring him to the party! #Francis #Voice:VA/GroupScene/BringHimToTheParty - else: Bring Sam too, once you find him! #Lorenzo #Voice:VA/GroupScene/BringSamToo}} //This is ugly as hell. (two lines in here)

* {dogmentioned} I still haven't found him. #Robin #Voice:VA/GroupScene/StillHaventFoundHim
    Tell us where you are Robin, we can come help. #Francis #Voice:VA/GroupScene/TellUsWhereYouAre
    Yeah, we'll make sure you and Sam get home safe! #Lorenzo #Voice:VA/GroupScene/WellMakeSure
    -> home_soon
* {!dogmentioned} I've been trying to find my dog. #Robin #Voice:VA/GroupScene/TryingToFindSam
    Sorry to hear that! #Lorenzo #Voice:VA/GroupScene/SorryToHearThat
    How long has Sam been missing? #Francis #Voice:VA/GroupScene/HowLongHasSamBeenMissing
    He ran off on me a little while ago. #Robin #Voice:VA/GroupScene/RanOffOnMe
    You've been out for more than a little while Robin, is Sam the only thing bothering you? #Lorenzo #Voice:VA/GroupScene/MoreThanAFewDays
    Yeah, it's closer to a few days, so I'm a little worried about you myself. #Francis #Voice:VA/GroupScene/CloserToAFewDays
    ->home_soon
* I just need a little bit longer to myself. #Robin #Voice:VA/GroupScene/ALittleLongerToMyself
    Robin.. we're worried about you. #Francis #Voice:VA/GroupScene/WereWorriedAboutYou
    We haven't seen you in a few days now... #Lorenzo #Voice:VA/GroupScene/HaventSeenYouInAFewDays
    I know, I'm sorry guys, but I'll be back soon. #Robin #Voice:VA/GroupScene/IllComeBackSoon
    We trust you Robin, but please come back safe and sound? #Lorenzo #Voice:VA/GroupScene/WeTrustYouRobin
    I will. #Robin #Voice:VA/GroupScene/IWill
    -> END

== home_soon ==
    I think I hear him just up ahead, I don't think I'll be much longer. #Robin #Voice:VA/GroupScene/HearHimJustUpAhead
    Alright, we'll wait, but just come back soon. #Francis #Voice:VA/GroupScene/ComeBackSoon
    Don't worry, I'll be home soon. #Robin #Voice:VA/GroupScene/DontWorry
    -> END
//Group Dialog End

//Police Start
== police_dialog ==
    This is an emergancy Missing Person Alert #Police #Voice:VA/CopDialog/MissingPerson
    The current whereabouts of Robin Wilf is unknown, and anyone with information is urged to report it to the nearest authorities. #Police #Voice:VA/CopDialog/CurrentWhereabouts
    Robin's mother noticed that they hadn't been home for a few days and reached out to Robin's friends, who haven't heard from them either. #Police #Voice:VA/CopDialog/RobinsMother
    It is believed that Robin was got lost after taking their dog, Sam for a walk. #Police #Voice:VA/CopDialog/ItIsBelieved
    The authorities are combing the areas near the Wilf residence, but no luck in the search just yet. #Police #Voice:VA/CopDialog/TheAuthorities
    Rest assured, the authorities will not rest until every acre is covered. #Police #Voice:VA/CopDialog/RestAssured
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
    Yeah, I started slipping through time and had to figure out a way through the forest. #Robin #Voice:VA/FoundSam/JourneyExplanationPart1 #waitForTrigger
    I redirected a river with a log, jumped some pretty large gaps and even managed to put out a fire! #Robin #Voice:VA/FoundSam/JourneyExplanationPart2
    Arf! #Sam #Voice:VA/SamBarks/Bark4 #waitForTrigger
    Yeah, I'm a little tired. #Robin #Voice:VA/FoundSam/ALittleTired
    ... #Robin
    Ding! #notificationSound #Delay:0.5
    Oh... #Robin #Voice:VA/FoundSam/OhNotif #Delay:0.5
    Actually, on second thought, I think I'm good for a little while. #Robin #Voice:VA/FoundSam/OnSecondThought #Delay:1.0 #waitForTrigger
    I'd rather not lose you again, Sam, one time is enough for me. #Robin #Voice:VA/FoundSam/IdRatherNotLoseYouAgain
    Woof! #Sam #Voice:VA/SamBarks/Bark3
    I'm glad you agree, let's go home. #Robin #Voice:VA/FoundSam/ImGladYouAgree
    Bark, bark! #Sam #Voice:VA/SamBarks/Bark7

    -> END


//Ending Start
== end_scene ==
    Woof! #Sam #Voice:VA/SamBarks/Bark2 #waitForTrigger
    I'm home.. #Robin #Voice:VA/ArriveHome/ImHome #waitForTrigger
    Robin where were you for so long! Ne... never mind. I'm just happy you're home! #Mom #Voice:VA/ArriveHome/ImHappyYoureHomeAngryVer
    I asked your friends, called the school, no one had seen you. #Mom #Voice:VA/ArriveHome/AskedEveryone
    {not (dialog1chat and dialog2chat): I even called the police I was so worried! #Mom #Voice:VA/ArriveHome/IEvenCalledThePolice}
    I'm sorry Mom, I got a little lost. #Robin #Voice:VA/ArriveHome/ImSorryMom
    \*sigh* It's okay dear, I'm just glad you came home. #Mom #Voice:VA/ArriveHome/ImjustGladYoureHome
    Arf! #Sam #Voice:VA/SamBarks/Bark2
    Of course you too, Sammy. #Mom #Voice:VA/ArriveHome/YouTooSammy
    Woof! #Sam #Voice:VA/SamBarks/Bark1
    ... #Robin
    I love you, Mom. #Robin #Voice:VA/ArriveHome/ILoveYouMom
    I love you too, sweetie. #Mom #Voice:VA/ArriveHome/ILoveYouTooSweetie
    ... #Delay:3.0
    The End #ReturnToMainMenu
    -> END

//Dialogs End


// Notifications Start
== random_notif ==
// Randomizes notifs
{~->reddit1|->reddit2|->reddit3|->reddit4|->insta1|->insta2|->insta1|->insta2|->news1|->news2|->news3|->news4|->news5}

//  Notifs
== reddit1 ==
My husband hand-delivered our baby boy at Winterburn Costco? #Notification:Readit #Voice:VA/Notifications/CostcoBaby
// Does the baby get a free membership?
-> END

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
-> END

== insta2 ==
{~plextrongames #Voice:VA/Notifications/PlexTron|yeg.news #Voice:VA/Notifications/YEG|WorkaholicWisp #Voice:VA/Notifications/WorkaholicWisp|Velvet.Tides #Voice:VA/Notifications/VelvetTides|itsmarinalow #Voice:VA/Notifications/ItsMarinaLow} has posted for the first time in a while. #Notification:Instancegram
-> END

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
-> END

== news5 ==
New rollercoaster parts spotted, potentially the new coaster in Galaxyland? #Notification:News #Voice:VA/Notifications/GalaxyLandCoaster
-> END

// Repeatable responses
== ohboy ==
// Oh boy..
#Voice:VA/Notifications/OhBoy
-> END

== thisisneat ==
// Hey, this seems interesting.
#Voice:VA/Notifications/ThisSeemsInteresting
-> END

== animalreply ==
//That animal is adorable!
#Voice:VA/Notifications/CuteAnimal
-> END
// Notifications End

// Barks Start
== bark_feed_sam ==
I should go to the kitchen, I need to feed Sam.
-> END

== bark_kibble ==
Sam's kibble is there in the corner.
-> END

== bark_bowl ==
Where did I put Sam's bowl again?
-> END

== bark_reception ==
I wonder what's happening online. I might get reception somewhere else.
-> END

== bark_gate ==
I need to get past that gate.
-> END

== bark_gate_locked ==
Why does the gate have to be locked.
-> END

== bark_fire_spread ==
What if the fire spreads.
-> END

== bark_avoid_fire ==
I don't want to go near those flames.
-> END

== bark_uneven_ground ==
The ground isn't flat here, the bucket just falls.
-> END

== bark_slow_fill ==
The bucket is filling... vveeerrryyy slllloooowwwwlllyyy.
-> END

== bark_slow_fill_hours ==
It will take a couple hours for this bucket to fill.
-> END

== bark_could_jump ==
I think I could jump over that ledge.
-> END


== bark_big_jump ==
That's a pretty big jump.
-> END

== bark_jump_momentum ==
I need to get moving faster.
-> END

== bark_fast_water1 ==
The water is too fast, I don't want to get near it.
-> END

== bark_fast_water2 ==
The water is too fast, it will just wash away.
-> END

== bark_cant_cross ==
I can't cross the water right now.
-> END

== bark_log_blocks_river ==
It looks like that log is blocking the river.
-> END

== bark_hear_barking ==
Is that barking?
-> END

== bark_jump_up ==
I think I could jump up that.
-> END

== bark_sam_follow ==
I'm sure Sam will follow me.
-> END

== bark_get_home ==
I need to get home
-> END

== bark_good_dog ==
Who's a good dog?
-> END

// Barks End
