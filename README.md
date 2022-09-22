## BethesdaSoftworksArchive MorrowindXBox
Requires .NET Framework 4.8

### Features
 - Can pack BSA archives for XBox properly.
 - Allows for modification of assets on Morrwind XBox.
 - (Potentially) allows for mod assets to be packed into working BSA for Morrowind XBox.
 - CLI tool allows for dragging folder onto executable and getting archives packed straight away.
 - CLI tool allows for dragging BSA onto executable and getting BSA unpacked.
 - Supports name table or non-name table BSA.

### F.A.Q

Q: _How can we **open BSA**?_  
A: The [BSA Browser](https://github.com/AlexxEG/BSA_Browser) tool by AlexxEG (see credits) supports opening Morrowind XBox BSA.  

Q: _Can this unpack BSA?_  
A: Yes. Although, it is recommended you use BSA Browser (see above question) as it is maintained frequently.  

Q: _How should I pack a BSA for Morrowind XBox?_  
A: Simply drag your 'Data'/'Data Files' folder onto the Packer CLI executable and it will pack it into a BSA which will work on XBox.  

Q: _Can this repack the game's main BSA?_  
A: Yes. This seems to be the only way to load additional content via a BSA for Morrowind XBox.  

Q: _Should I pack BSAs for PC with this?_  
A: Yes although I reccommend for PC to use another tool like [bsapack](https://github.com/xyzz/bsapack). This tool can pack for PC Morrowind perfectly fine.  

Q: _Why is BSA Browser able to read the BSA's file names that I just packed? Shouldn't there be no name table?_  
A: If dragging a folder onto the executable to pack, by default, a name table will be generated in the BSA. XBox Morrowind should still load this fine, this was done to prevent potential data loss (not knowing what file is what) when opening a packed BSA. If this is not desired, simply packing from the executable without the '-strtable' command will do the trick (please look at executable help menu).  

Q: _I am trying to build/debug this program, why am I not getting file names when unpacking a BSA?_  
A: Check out the [DOCUMENTATION](https://github.com/SockNastre/BethesdaSoftworksArchive-MorrowindXBox/tree/main/__DOCUMENTATION__) folder for a hash table file to use. Put this file in the same folder as executable.  

### Credits
 - Thanks to [asorrycanadian](https://github.com/asorrycanadian) for helping out where needed, testing, and back-and-forth dialog.
