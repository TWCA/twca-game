-> checkup_text

== checkup_text ==
Hi Robin, just checking in on you, it's been a while since we last talked! #Friend #F1

 * [...] #Robin #R1A
 -> checkup_end 
 * I've been okay. #Robin #R1B
   Oh? Something happening? #Friend #F2A
   * * Sam's gone missing, and I haven't found him yet. #Robin #R2A
       Sorry to hear that Robin! Anything I can do? #Friend #F3A
       No, I'm sure I'll find him soon, I'm okay. #Robin #R3A
       Alright then, good luck! #Friend #F4A
       Oh and, -> checkup_end 
   * * It's just been a bit busy lately. #Robin #R2B
       Fair enough. Anyways, just wanted to check in, make sure you're doing well! #Friend #F3B
       -> checkup_end
 * I've been good. #Robin #R1C
   That's great to hear! #Friend #F2B
   So, -> checkup_end 

== checkup_end ==
I know you've been busy the last few times I tried to invite you out, but we'll hang out this time, okay? #Friend #FF
 * [...] #Robin #RF1
    -> END
 * Okay. #Robin #RF2
    -> END
