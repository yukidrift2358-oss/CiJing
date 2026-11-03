# v0.5 音乐与采样来源

采样：VSCO 2 Community Edition，Versilian Studios / Sam Gossner；按项目 README 一并致谢 Ivy Audio / Simon Dalzell。
原始项目：https://github.com/sgossner/VSCO-2-CE
许可：CC0 1.0，附 VSCO-LICENSE.txt 和 VSCO-Readme.txt。

选用 Strings/Harp/KSHarp_D4_mf.wav、KSHarp_A4_mf.wav，以及 Woodwinds/Flute/expvib/LDFlute_expvib_C4_v1_1.wav。

处理：单声道转换、按十二平均律重采样为 C4/D4/E4/G4/A4、峰值统一为 0.65、短/长余韵裁剪与淡出。竖琴是竖琴录音，不作为古琴或古筝采样。

配乐：本项目原创的三组五音乐句，使用上述采样编排为每章 44.8 秒循环。包含分解音型、低音与长笛句，峰值 0.60。不是下载的成品歌曲。主音演奏时自动降低配乐音量，音石与配乐分别调节。

处理脚本保留在 Docs/AudioSource/prepare_audio.py。需 numpy 和 imageio-ffmpeg；这些开发依赖不随游戏或工程压缩包打包。
