## BethesdaSoftworksArchive MorrowindXBox
Requires .NET Framework 4.8

### Features
 - Can pack BSA archives for XBox properly.
 - Allows for modification of assets on Morrwind XBox.
 - (Potentially) allows for mod assets to be packed into working BSA for Morrowind XBox.
 - CLI tool allows for dragging folder onto executable and getting archives packed straight away.
 - CLI tool allows for dragging BSA onto executable and getting BSA unpacked.

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
A: No. For PC use another tool like [bsapack](https://github.com/xyzz/bsapack).  

Q: _I am trying to build/debug this program, why am I not getting file names when unpacking a BSA?_  
A: Check out the [DOCUMENTATION](https://github.com/SockNastre/BethesdaSoftworksArchive-MorrowindXBox/tree/main/__DOCUMENTATION__) folder for a hash table file to use. Put this file in the same folder as executable.  

### Credits
 - Thanks to [asorrycanadian](https://github.com/asorrycanadian) for helping out where needed, testing, and back-and-forth dialog.
 - Josip Medved for the OpenFolderDialog.