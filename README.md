# ResXGen

Use for Easy. C#.  
.NetFrameworkの"resgen.exe"の代替アプリ。  

# Features

ABCDE##XYZ

# Requirement

# Installation

# Usage

ResXGen.exe リソース定義.rc  

ResXGen.exe input.rc  
input.resxを作成します。  

# Note

<書式例>  
STRING    mystring1  "あういえおでん"   # 文字列例  
BITMAP    mybitmap1  "/imgs/pic1.bmp"  # ビットマップ例  
  
```
----+----1----+----2----+----3----+----4  
STRING  resid  "文字列"  String型  
TEXT    resid  "パス"    String型  
BITMAP  resid  "パス"    Bitmap型  
CURSOR  resid  "パス"    Bitmap型  
ICON    resid  "パス"      Icon型  
any     resid  "パス"      Byte型  
  
<サンプル> input.rc  
STRING  MYSTRING1 "あいうえおでん"  
ICON    myicon10    "fold/1.ico"  
BITMAP  MYBMP1      "fold/model_71.png"  
TEXT    sampletext  "fold/sample.txt"  
EXCEL   excel       "fold/a.xlsx"  
```

# Author

# License


Thank you!
