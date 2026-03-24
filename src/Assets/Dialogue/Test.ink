1
+[choice 1]->start

=== headless_test ===
...Just a few more minutes Sam... #Voice:VA/IntroScene/IntroRobin1
New Telus World of Science exhibit unveiled! #Voice:VA/Notifications/TelusWorldOfScience
I've been okay. #Voice:VA/InterLevel/IveBeenOkay
-> END

=== start ===
NPC: What u doing here？

+ [im passing by.]
    -> passerby
+ [im looking for TimePortal.]
    -> portal
+ [who ru？]
    -> who
+ [bye]
    -> bye

=== passerby ===
NPC: Ok, Don't stay too long.
-> loop

=== portal ===
NPC: TimePortal is dangerous, u sure？
+ [Yes]
    NPC: Fine, but don't back.
    -> END
+ [No]
    NPC: come when u ready.
    -> loop

=== who ===
NPC: Im just a reminder
-> loop

=== bye ===
NPC: Bye
-> END

=== loop ===
NPC: Anything else?
-> start
