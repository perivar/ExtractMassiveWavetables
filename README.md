# ExtractMassiveWavetables
Extract Native Instruments Wavetables from it's tables.dat file

## Extract the files from Native Instruments Massive tables.dat file.
This is ported over from python to c#
https://gist.github.com/lalinsky/8f2cd9e8f80e62c82af2
all credits goes to Lukáš Lalinský

*Usage*
```
ExtractMassiveWavetables.exe path-to-tables.dat path-to-output-directory
```

This will extract the content into two directories:
One with the original extracted content: Original
And one where the filenames are renamed to the same used by the NI Massive UI: "Correct Waveforms".

Read here for more info:
http://www.reddit.com/r/edmproduction/comments/2d2op8/massive_wavetables_list_info/