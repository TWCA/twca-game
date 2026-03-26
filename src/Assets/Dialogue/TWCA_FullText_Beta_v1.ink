/* 
-Index for coders-:

intro_scene: Intro scene, includes dog food section (Robin, Sam, Mom)

-Dialog Sections-:
dialog_1: Amelia & Robin (Semi-voiced, I added lines after the ends)
dialog_2: Lorenzo & Robin
dialog_3: Selects one of two options
-> group_dialog: Amelia and Lorenzo ask about Robin
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
Good morning Robin, I left you some toast on the table. # Mom
 * [...] # Robin
 * Thanks. # Robin
 - I've got a lot of work so I'll be out late tonight, either make yourself something or order in a dinner. # Mom
 Oh, and could you take Sam out for a walk, he gets too energetic when he's left inside too long. # Mom
 I will. # Robin
 Alright, see you later then, I'm off. # Mom
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
 ! #Ding!
 Hold on one second Sam, I have to check this notification. # Robin
 Those were some interesting posts, now where were we Sam? # Robin
 ... # Robin
 Sam? # Robin
-> END
// Intro End


//Dialogs Start

// Dialog 1 Start
== dialog_1 ==
Hi Robin, just checking in on you, it's been a while since we last talked! #Amelia #Voice:VA/InterLevel/HiJustChecking

 * I've been okay. #Robin #Voice:VA/InterLevel/IveBeenOkay
   Oh? Something happening? #Amelia #Voice:VA/InterLevel/SomethingHappening
   * * Sam's gone missing, and I haven't found him yet. #Robin #Voice:VA/InterLevel/SamsGone
       ~ dogmentioned = true
       Sorry to hear that Robin! Anything I can do? #Amelia #Voice:VA/InterLevel/SorryToHear
       No, I'm sure I'll find him soon, I'm okay. #Robin #Voice:VA/InterLevel/IllFindHim
       Alright then, good luck! #Friend #Voice:VA/InterLevel/GoodLuck
       Oh and, #Amelia #Voice:VA/InterLevel/OkIKnow #IgnoreNextVoice -> checkup_end_pos 
   * * It's just been a bit busy lately. #Robin #Voice:VA/InterLevel/ItsBeenBusy
       Fair enough. Anyways, just wanted to check in, make sure you're doing well! #Amelia #Voice:VA/InterLevel/FairEnough
       -> checkup_end_pos
 * I've been good. #Robin #Voice:VA/InterLevel/IveBeenGood
   That's great to hear! #Amelia #Voice:VA/InterLevel/GreatToHear
   So, #Amelia #Voice:VA/InterLevel/SoIKnow #IgnoreNextVoice -> checkup_end_pos
 * [...] #Robin
 -> checkup_end_neg 

== checkup_end_pos ==
~ dialog1chat = true
I know you've been busy the last few times I tried to invite you out, but we'll hang out this time, okay? #Amelia #Voice:VA/InterLevel/IKnow
 * Okay. #Robin #Voice:VA/InterLevel/Okay
    Sounds like a plan then! #Amelia
    -> END
 * [...] #Robin
    -> END
== checkup_end_neg ==
~ dialog1chat = false
I know you've been busy the last few times I tried to invite you out, but we'll hang out this time, okay? #Amelia #Voice:VA/InterLevel/IKnow
 * Okay. #Robin #Voice:VA/InterLevel/Okay
   I know you don't want to talk now, but I'll be here when you do. #Amelia
    -> END
 * [...] #Robin
   I'm worried about you Robin. #Amelia
    -> END
//Dialog 1 End


//Dialog 2 Start
== dialog_2 ==
Hey Robin, you weren't at class the other day, and I'll just been wondering how you've been? #Lorenzo
 * I'm just feeling a little sick. #Robin
   So that's why I haven't seen you around! #Lorenzo
   Make sure to rest up and feel better! #Lorenzo
    -> class_end_pos
 * Just dealing with a few things. #Robin
   Something to do with your mother? #Lorenzo
   * * No, Sam's just gone off on his own. #Robin
       That rebelious dog!!! #Lorenzo
       I'm sure you'll find him. #Lorenzo
       I hope so too. #Robin
       ~ dogmentioned = true
      -> class_end_pos
   * * I'd prefer to not say. #Robin
       All good Robin, I'm here to listen if you ever want to talk about it. #Lorenzo
      -> class_end_pos
 * [...]
    -> class_end_neg
   
== class_end_pos ==
   ~ dialog2chat = true
   Don't worry, I'll make sure my notes are nice and neat so you can borrow them and catch up! #Lorenzo
   Thanks, Lorenzo. #Robin
   Of course! #Lorenzo
    -> END
    
== class_end_neg ==
   ~ dialog2chat = false
   Whatever the reason, we'll be waiting for you Robin. #Lorenzo
   Just.. keep us in the loop? #Lorenzo
    -> END
//Dialog 2 End


//Select Dialog 3
== dialog_3 ==
{dialog1chat or dialog2chat: ->group_dialog}
->police_dialog


//Group Dialog Start
== group_dialog ==
Hey Robin, we were talking and we were hoping to have a little get-together soon. #Amelia
We know you've been away for a little while, but we'd like to see you again. #Lorenzo
{dogmentioned: {dialog1chat: If you found Sam, bring him to the party! #Amelia - else: Bring Sam too, once you find him! #Lorenzo}} //This is ugly as hell. (two lines in here)

* {dogmentioned} I still haven't found him. #Robin
    Tell us where you are Robin, we can come help. #Amelia
    Yeah, we'll make sure you and Sam get home safe! #Lorenzo
    -> home_soon
* {!dogmentioned} I've been trying to find my dog. #Robin
    Sorry to hear that! #Lorenzo
    How long has Sam been missing? #Amelia
    He ran off on me a little while ago. #Robin
    You've been out for more than a little while Robin, is Sam the only thing bothering you? #Lorenzo
    Yeah, it's closer to a few days, so I'm a little worried about you myself. #Amelia
    ->home_soon
* I just need a little bit longer to myself. #Robin
    Robin.. we're worried about you. #Amelia
    We haven't seen you in a few days now... #Lorenzo
    I know, I'm sorry guys, but I'll be back soon. #Robin
    We trust you Robin, but please come back safe and sound? #Lorenzo
    I will. #Robin
    -> END

== home_soon ==
    I think I hear him just up ahead, I don't think I'll be much longer. #Robin
    Alright, we'll wait, but just come back soon. #Amelia
    Don't worry, I'll be home soon. #Robin
    -> END
//Group Dialog End

//Police Start
== police_dialog ==
    Missing Person Alert #Police
    Current whereabouts of Robin Wilf is unknown, and anyone with information is urged to report it to the nearest authorities. #Police
    Robin's mother noticed that they hadn't been home for a few days and reached out to Robin's friends, who haven't heard from them either. #Police
    It is believed that Robin was got lost after taking their dog, Sam for a walk. #Police
    The authorities are combing the areas near the Wilf residence, but no luck in the search just yet. #Police
    And to Robin, if you're hearing this, please come home, everyone's worried about you. #Police
-> END
//Police End


//Meet Sam
== meet_sam ==
    Sam! There you are! #Robin
    Woof! #Sam
    I've been all over looking for you. #Robin
    I'm sorry I lost track of time and left you on your own. #Robin
    But we're together again now, so let's head home. #Robin
    Arf! #Sam
    -> END


//Walk
== walk_home ==
    You would not believe what I've been through getting here Sam. #Robin
    Woof? #Sam
    Yeah, I started slipping through time and had to figure out a way through the forest. #Robin
    I redirected a river with a log, jumped some pretty large gaps and even managed to put out a fire! #Robin
    Arf! #Sam
    Yeah, I'm a little tired. #Robin
    ... #Robin
    ! #Ding
    Oh a notification... #Robin
    Actually, on second thought, I think I'm good for a little while. #Robin
    I'd rather not lose you again, Sam, one time is enough for me. #Robin
    Woof! #Sam
    I'm glad you agree. #Robin
    Now, let's get home! #Robin
    Bark, bark! #Sam

    -> END


//Ending Start
== end_scene ==
    I'm home.. #Robin
    Oh Robin, I'm so happy you're home! #Mom
    Where were you, for so long? #Mom
    I asked your friends, called the school, no one had seen you. #Mom
    {not (dialog1chat and dialog2chat): I even called the police I was so worried! #Mom}
    I'm sorry Mom, I got a little lost. #Robin
    \*sigh* It's okay dear, I'm just glad you came home. #Mom
    Arf! #Sam
    Of course you too, Sammy. #Mom
    Woof! #Sam
    ... #Robin
    I love you, Mom. #Robin
    I love you too, sweetie. #Mom
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
