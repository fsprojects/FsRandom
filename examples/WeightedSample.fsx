#if INTERACTIVE
#I "../Build/lib/net45"
#r "FsRandom.dll"
#endif

open FsRandom
// from https://www.scb.se/hitta-statistik/statistik-efter-amne/befolkning/amnesovergripande-statistik/namnstatistik/pong/tabell-och-diagram/samtliga-folkbokforda--efternamn-topplistor/efternamn-topp-100/
let be0001namntab40_2019=[
    "Andersson",228_264
    "Johansson",227_104
    "Karlsson",202_331
    "Nilsson",155_686
    "Eriksson",136_533
    "Larsson",114_698
    "Olsson",103_689
    "Persson",98_019
    "Svensson",91_741
    "Gustafsson",89_534
    "Pettersson",87_169
    "Jonsson",68_358
    "Jansson",45_844
    "Hansson",40_517
    "Bengtsson",31_649
    "Jönsson",28_874
    "Lindberg",27_119
    "Jakobsson",25_669
    "Magnusson",25_109
    "Olofsson",24_493
    "Lindström",24_321
    "Lindqvist",22_461
    "Lindgren",22_250
    "Axelsson",21_671
    "Berg",21_525
    "Bergström",20_739
    "Lundberg",20_665
    "Lind",20_305
    "Lundgren",19_995
    "Lundqvist",19_617
    "Mattsson",18_926
    "Berglund",18_706
    "Fredriksson",17_691
    "Sandberg",17_406
    "Henriksson",16_908
    "Forsberg",16_335
    "Sjöberg",16_187
    "Wallin",15_831
    "Ali",15_473
    "Engström",15_320
    "Mohamed",15_253
    "Eklund",15_097
    "Danielsson",14_898
    "Lundin",14_755
    "Håkansson",14_545
    "Björk",14_200
    "Bergman",14_067
    "Gunnarsson",14_017
    "Holm",13_897
    "Wikström",13_738
    "Samuelsson",13_643
    "Isaksson",13_474
    "Fransson",13_432
    "Bergqvist",13_254
    "Nyström",13_051
    "Holmberg",12_892
    "Arvidsson",12_862
    "Löfgren",12_655
    "Söderberg",12_435
    "Nyberg",12_368
    "Blomqvist",12_226
    "Claesson",12_067
    "Nordström",11_969
    "Mårtensson",11_717
    "Lundström",11_527
    "Ahmed",11_431
    "Viklund",11_287
    "Björklund",11_187
    "Eliasson",11_187
    "Pålsson",11_112
    "Hassan",11_061
    "Berggren",11_016
    "Sandström",10_676
    "Lund",10_526
    "Nordin",10_514
    "Ström",10_299
    "Åberg",10_283
    "Hermansson",10_157
    "Ekström",10_136
    "Falk",10_054
    "Holmgren",9_966
    "Dahlberg",9_805
    "Hellström",9_784
    "Hedlund",9_749
    "Sundberg",9_696
    "Sjögren",9_628
    "Ek",9_473
    "Blom",9_413
    "Abrahamsson",9_310
    "Martinsson",9_270
    "Öberg",9_254
    "Andreasson",9_024
    "Strömberg",8_930
    "Månsson",8_896
    "Åkesson",8_745
    "Hansen",8_673
    "Norberg",8_587
    "Lindholm",8_578
    "Dahl",8_563
    "Jonasson",8_520
]
let weights = List.map snd be0001namntab40_2019 |> List.map float |> List.toArray
let names = List.map fst be0001namntab40_2019 |> List.toArray
let n = Array.length weights
let randomNames = Array.weightedSample n weights names

let seed = 123456789u, 362436069u, 521288629u, 88675123u
let state = createState xorshift seed
Random.get randomNames state |> Seq.take 20 |> Seq.iter (printfn "%s")
