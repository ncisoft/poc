With this utility, you could find out the memory layout, in Linux, the memory layout is:
 [ memory low ]
 [code]
 [const]
 [heap]
 [shared library]
 [stack]
 [ memory high ]

Using this facts, we are able to determine whether we are in the main coroutine, or individual coroutine, even we can determine which
coroutine we are
