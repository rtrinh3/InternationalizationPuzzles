#!/usr/bin/env python3

# https://i18n-puzzles.com/puzzle/19/
# Puzzle 19: Out of date

import os
import sys
import zoneinfo
import datetime
import urllib.request

def Solve(inputFileName: str):
    # Prepare tz
    tzVersions = ['2018c', '2018g', '2021b', '2023d']
    for version in tzVersions:
        if not os.path.isdir(version):
            tzFileName = 'tzdata{0}.tar.gz'.format(version)
            tzUrl = 'https://data.iana.org/time-zones/releases/' + tzFileName
            urllib.request.urlretrieve(tzUrl, tzFileName)
            os.system('tar -xf tzdata{0}.tar.gz --one-top-level'.format(version))
            os.system('zic -d {0} tzdata{0}/africa tzdata{0}/antarctica tzdata{0}/asia tzdata{0}/australasia tzdata{0}/etcetera tzdata{0}/europe tzdata{0}/northamerica tzdata{0}/southamerica'.format(version))

    # Parse
    inputDatesByPlace = dict()
    with open(inputFileName) as inputFile:
        for line in inputFile:
            parts = line.split(';', 1)
            inputDatesByPlace.setdefault(parts[1].strip(), []).append(parts[0].strip())

    # Shift
    # shifted[place][version] = [list of datetime]
    shifted = dict()
    for version in tzVersions:
        zoneinfo.ZoneInfo.clear_cache()
        zoneinfo.reset_tzpath([os.path.abspath(version)])
        for zoneId, timestamps in inputDatesByPlace.items():
            zoneContainer = shifted.setdefault(zoneId, dict())
            shiftTimes = zoneContainer.setdefault(version, [])
            zone = zoneinfo.ZoneInfo(zoneId)
            for timestamp in timestamps:
                dt = datetime.datetime.fromisoformat(timestamp)
                dtLocal = dt.replace(tzinfo=zone)
                dtUtc = dtLocal.astimezone(datetime.timezone.utc)
                dtUtcStr = dtUtc.isoformat()
                shiftTimes.append(dtUtcStr)

    # Find overlap
    places = list(shifted.keys())
    stack = []
    stack.append((None, tuple()))
    while len(stack) > 0:
        (dates, selectedZones) = stack.pop()
        index = len(selectedZones)
        if index == len(places):
            # print(selectedZones)            
            return dates
        else:
            for variant, variantDates in shifted[places[index]].items():
                newDates = dates.intersection(variantDates) if dates != None else frozenset(variantDates)
                if len(newDates) > 0:
                    newZones = selectedZones + ('{0} ({1})'.format(places[index], variant),)
                    stack.append((newDates, newZones))
    return []

if __name__ == '__main__':
    if len(sys.argv) <= 1:
        print("Missing argument")    
        sys.exit()
    dates = Solve(sys.argv[1])
    for d in dates:
        print(d)
