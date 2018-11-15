# Project Salieri
## What is this project?
Get contents from various sources and analyze

## Features
- Salieri.Core
  - Gather data from websites
  - Data preparation for analysis
  - Analysis
    - Word Cloud
    - word2vec
- SalieriCLI
  - Command Line Interface to use Salieri.Core
- SalieriWeb
  - Web based visual UI to use Saloeri.Web  

## Why the project name is "Salieri"?
The name came from [Antonio Salieri](https://ja.wikipedia.org/wiki/%E3%82%A2%E3%83%B3%E3%83%88%E3%83%8B%E3%82%AA%E3%83%BB%E3%82%B5%E3%83%AA%E3%82%A8%E3%83%AA)  
In the Japanese Animation [Steins;Gate Zero](http://steinsgate0-anime.com/), there is an AI named Amadeus, which is named after [Wolfgang Amadeus Mozart](https://ja.wikipedia.org/wiki/%E3%83%B4%E3%82%A9%E3%83%AB%E3%83%95%E3%82%AC%E3%83%B3%E3%82%B0%E3%83%BB%E3%82%A2%E3%83%9E%E3%83%87%E3%82%A6%E3%82%B9%E3%83%BB%E3%83%A2%E3%83%BC%E3%83%84%E3%82%A1%E3%83%AB%E3%83%88)  
I love the animation, so I first thought to name this project "Amadeus".  
But Amadeus is so perferct, I decided to keep the project name "Amadeus" for future use. When I come up really brilliant idea, I will name it "Amadeus"  

For this project, I choose another great music compoer, but not as great as Wolfgang Amadeus Mozart. That is "Antonio Salieri"  

## USAGE
### Sarieli CLI
Search Keyword "Microsoft Azure" on Google and get the content of top 100 results  
```
SalieriCLI "Microsoft Azure"  
```
Once Analyzed, world list with occurence will be shown  
```
[2450]  azure  
[1293]  cloud  
[1233]  microsoft  
[1097]  service  
[1006]  data  
[486]   use  
[415]   support  
[384]   solution  
[373]   application  
[362]   learn  
[361]   management  
[313]   s  
[312]   storage  
[305]   can  
[302]   product  
[286]   security  
[282]   1  
[277]   resource  
[274]   platform  
[271]   machine  
[264]   business  
```
You can find simlar words
```
Enter keyword to analyze and press enter
machine

Word: machine  Position in vocabulary: 50
linux   0.9990176
products        0.9986746
get     0.9985769
workloads       0.9984719
at      0.9984297
windows 0.9984263
virtual 0.9983898
ai      0.998272
learning        0.9981397
tools   0.9978774

Press Enter to exit
```
