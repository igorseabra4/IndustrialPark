TxdGen 1.0 by The_GTA (wordwhirl@outlook.de)
==================================

TxdGen is the RW Texture Dictionary conversion tool. Its goal is to support
every known TXD format and to allow conversion between them.

I began working on this tool after getting the brilliant idea to mod the
PlayStation 2 version of San Andreas. I knew that there was nothing
stable out there yet, so I see this as an opportunity to contribute great
tools to the GTA community. This is for you, modders!

Never has the conversion between TXD formats been so easy and bugfree.
Simply put the TXD containers in one folder and the tool will generate
the output into another folder.

Fix problems that the Rockstar development team has overlooked! The parser
comes with rich debugging output to inform you about any problems that
lurk inside of official and community TXD files.

Now supports GTA3 Android/iOS/mobile TXDs! Using a freshly rewritten RenderWare
implementation TxdGen is one of the most stable RW file converters out there!

*** October 2016: Now comes support for even more TXD formats such as Gamecube
and Rockstar Leeds PSP! The TXD framework is being shared with the Magic.TXD tool
which you can find at:

http://gtaforums.com/topic/851436-relopensrc-magictxd/

Feedback at both ends is very appreciated.
I stopped shipping the imgimport executable. Instead I recommed using IMG Factory by X-Seti.

http://www.gtagarage.com/mods/show.php?id=27155

==================================
REQUIREMENTS
==================================
Windows XP and above. No runtime library requirements anymore.

==================================
HOW TO USE TXDGEN
==================================
0) configure "txdgen.ini"
1) download a TXD mod from GTAGarage (i.e. "Sexy Miku Wall")
2) put the .txd files into the "txdgen_in/" folder
3) execute "txdgen.exe"
4) take the new .txd files from the "txdgen_out/" folder
5) place the new .txd files into your custom game (PS2, XBOX, etc)

For more information and tutorials, see
http://www.gtamodding.com/wiki/TxdGen
http://www.gtamodding.com/wiki/Magic.TXD

==================================
CREDITS
==================================
1) uses a fork of rwtools by aap (https://github.com/aap/rwtools)
2) uses the libimagequant library (http://pngquant.org/lib/)
3) uses the libsquish library (https://code.google.com/p/libsquish/)
4) uses the lzo library (http://www.oberhumer.com/opensource/lzo/)
5) Thanks to aru from GTAForums for XBOX swizzling and unswizzling algorithms (http://gtaforums.com/topic/213907-unswizzle-tool/)
6) Some parts are inspired by research from DK22Pac
7) Uses the PowerVR SDK (http://www.imgtec.com/tools/powervr-tools/)
8) Uses the Compressonator library (https://github.com/GPUOpen-Tools/Compressonator)
9) uses the libjpeg library (http://libjpeg.sourceforge.net/)
10) uses the libpng library (http://libpng.org/pub/png/libpng.html)

==================================
LIMITATIONS
==================================
This tool is not made for automatically adjusting the texture size so
that it will be optimized for a specific platform. The creator of the TXD
has to give you an optimized version instead.

If you want to author TXD files, use Magic.TXD instead.

==================================
COMMON PITFALLS
==================================
* make sure your paths end in a trailing slash!

==================================
LICENSES
==================================
By using this tool you agree to licenses that are shipped with it. They
are placed inside of the "LICENSES/" folder. I include them out of respect
to the people that virtually contributed to my work.

==================================
SOURCE CODE
==================================
The source code to this project is located on the MTA:Eir green-candy project space.
It is in its own repository under:

https://app.assembla.com/spaces/green-candy/subversion-19/source

- October 2016

PS: if you like a great community, give mtasa.com a try. when I get
nice stuff to the GTAForums community, I will return to my MP project,
I promise :)

Check my YouTube channel (rplgn) at
https://www.youtube.com/channel/UCfZ9ZHSNkk5xnMW8Db1G-Vw