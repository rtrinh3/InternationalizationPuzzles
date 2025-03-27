#!/usr/bin/env python3

# https://i18n-puzzles.com/puzzle/19/
# Puzzle 19: Out of date

import Puzzle19

tests = [
    ('19-test-input.txt', '2024-04-09T17:49:00+00:00'),
    ('19-input.txt', '2024-04-09T03:40:00+00:00')
]

for t in tests:
    (testFile, expected) = t
    print('Testing ' + testFile)
    result = Puzzle19.Solve(testFile)
    for d in result:
        print(d)
    if len(result) == 1 and expected in result:
        print('OK')
    else:
        print('Fail')
