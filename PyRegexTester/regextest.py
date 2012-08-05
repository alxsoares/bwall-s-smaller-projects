#!/usr/bin/env python

import curses
import re

data = "hello world"
regex = "(.*)"
results = ""
cursorRow = 0
cursorCol = 0
current = data
x = 0

while 1:
    screen = curses.initscr()
    
    screen.keypad(1)
    height = screen.getmaxyx()[0];
        
    if cursorRow == 0:
        current = regex
    elif cursorRow == 1:
        current = data

    screen.clear()
    screen.border(0)
    screen.addstr(1, 2, "regex:" + regex)
    screen.addstr(2, 2, "data:" + data)
    screen.addstr(3, 2, "results:" + results)
    screen.addstr(height - 2, 2, "@bwallHatesTwits - Python regex tester v0.1 (Ctrl-g to quit)")
    try:
        m = re.search(regex, data)
        if m:
            index = 0
            for g in m.groups():
                screen.addstr(4 + index, 3, "group " + str(index) + ":" + g)
                index += 1
    except:
        results = ""
    screen.move(1 + (1 * cursorRow), 2 + 6 + cursorCol - cursorRow)
    screen.refresh()
     
    x = screen.getch()
    screen.keypad(0)
    if x == 7:
        break
    elif x == 260:
        #go left
        if cursorCol != 0:
            cursorCol -= 1
    elif x == 261:
        #go right
        if cursorCol < current.__len__():
            cursorCol += 1
    elif x == 263:
        #bckspace
        if cursorCol != 0:
            if cursorCol == current.__len__():
                current = current[:cursorCol - 1];
            else:
                current = current[:cursorCol - 1] + current[cursorCol:]
            cursorCol -= 1
            if cursorRow == 0:
                regex = current
            else:
                data = current
    
    elif x == 330:
        #delete
        if cursorCol < current.__len__():
            if cursorCol == 0:
                current = current[cursorCol + 1:]
            else:
                current = current[:cursorCol] + current[cursorCol + 1:]  
            if cursorRow == 0:
                regex = current
            else:
                data = current 
    elif x == 360:
        #end
        cursorCol = current.__len__()

    elif x == 262:
        #home
        cursorCol = 0                
                
    elif x == 259:
        cursorRow = 0
        if cursorCol > regex.__len__():
            cursorCol = regex.__len__()
    elif x == 258:
        cursorRow = 1
        if cursorCol > data.__len__():
            cursorCol = data.__len__()
    elif x >= 32 and x <= 126:
        current = current[:cursorCol] + chr(x) + current[cursorCol:]
        cursorCol += 1
        if cursorRow == 0:
            regex = current
        else:
            data = current

curses.endwin()
