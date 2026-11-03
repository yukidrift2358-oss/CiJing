# 词境 v0.13

Unity 2022.3.62f3c1 / URP 2D / uGUI。三章九景的古风二合休闲解谜 Demo。

本版将音石演奏接入风、雨、水面、帘与舟的状态变化，留白参与场景的消退与发现。卷帘增加落笺和留花两条解法，后续回信随之变化。可逐拍回看和续听，自由谱曲可保存曲目与场景。

详细操作见 [试玩说明](Docs/v013-试玩说明.md)，规则、资源、制作方法和验证边界见 [音景联动与制作记录](Docs/v013-音景联动与制作记录.md)。旧版迭代文档仍在 Docs 中保留。

入口场景：Assets/Scenes/CiJing.unity。批处理构建入口：CiJing.EditorTools.BuildGame.Run，默认输出到工程同级 Deliverables/CiJing-v0.13-Windows。该入口会重新创建入口场景。

Assets、Packages 和 ProjectSettings 可直接用于打开工程；正常运行不需要重新制作音频素材。开发用音频脚本在 Docs/AudioSource，需另装 Python 与 numpy 等依赖；开发录制工具所需 FFmpeg 不随源码压缩包附带。

Windows Player 的 --self-test 使用独立 test-save.json；--audio-test 检查音量控制。正式进度保存为 journey-v1.json，位于 %USERPROFILE%/AppData/LocalLow/CiJingStudio/词境/。自动操作验证不计作真人体验测试。

李清照词句与自撰题句在游戏中分别标明。场景和机关为词意改编，不是生平、宋代音乐或古代生活的考据复原。美术含 AI 生成的水墨素材；演出为 Unity 原生二维分层，没有使用 Live2D Cubism 模型。

录音采样、许可与致谢见 Docs/Audio-Credits-v05.md 和 VSCO-LICENSE.txt；本版配乐采用原采样重新编排，风雨纹理由项目制作。
