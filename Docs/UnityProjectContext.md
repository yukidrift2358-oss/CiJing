# Unity 项目上下文

独立新建项目 CiJing，与同目录的 LastLightStation 等其他项目无关；sources 为同步只读资料，未作更改。

Unity：2022.3.62f3c1。创建来源：本机 universal-2d 模板。URP 14.0.12；uGUI；Legacy Input 用于键盘，EventSystem 用于指针。没有额外安装 MCP 桥接，使用 Unity 编辑器批处理构建与实际 Windows Player 验证。

Assets/Game/Runtime/GameModel.cs：纯规则、九关内容、棋盘与存档结构。
Assets/Game/Runtime/GameApp.cs：界面与流程、音频、存档、输入和运行验证入口。
Assets/Game/Runtime/AtmosphereGraphic.cs：轻量场景动态表现。
Assets/Game/Editor/BuildGame.cs：规则断言、场景创建、播放器设置与 Windows 构建。
Assets/Resources/Art：三张 AI 绘制背景。
Assets/Resources/Fonts/Poetry.ttf：实际使用的中文字体。

入口场景：Assets/Scenes/CiJing.unity。构建方法会重新创建该场景；手工修改该场景前，应将所需改变同步到构建方法，或改用直接 Build Settings 构建。

后续维护优先将音频、存档与页面构建进一步拆分为独立组件；当前版本保持少量文件以方便阅读原型。不要把自动验证视作真人体验验证。


## v0.13 音景联动

SceneScore.cs：确定性的逐拍环境状态；SceneMoment 可重算，寻拍与完整演奏采用同一演算。SceneScoreValidation.cs 为编辑器规则检查。

GameApp.SceneScore.cs：景物显示的连接、拍位回看、续听、完整演奏判定、画记、自由取音和新增运行检查。ScoreSceneView.cs 负责分层纱帘、雨波、落瓣与舟身显示。SceneArt.shader 负责渐变设色、花色退去和透明边缘。

SaveData 新增 sceneOutcomes、sceneDiscoveries、compositionScene、compositionChapter，读取旧存档时补齐数组与默认值；原有 journey-v1.json 路径不变。

MusicBed 使用三组同步双层配乐、风雨环境层、长笛回应。真实录音与本版编排说明在 Docs/v013-音景联动与制作记录.md。正常构建无需运行音频脚本。

BuildGame 默认输出 CiJing-v0.13-Windows。保留 --film-build 开发分支；此前成片属于 v0.12，不能作为 v0.13 新功能的运行证据。


## v0.14 引导与小卷

GameApp.Guidance.cs：按操作显示的主题引导、GuideFocus 非阻挡墨圈、独立 Board 小卷练习、关闭取消及新增自动检查。guideDone 和 guideDismissed 分开保存，旧存档默认未学习，不重置主线。

--guide-test 可单独检查练习与提示；--self-test 包含九关及新增引导检查。v0.14 默认构建输出 CiJing-v0.14-Windows，旧发布包保留。


## v0.15 景物目标与回访

GardenGoals.cs 负责第二章三个景物目标；VerseRules 仍是统一判定入口，第二章直接交给景物目标，首尾不再按圆环旋转等价处理。SceneMoment 新增雨滴、风阵与隔帘听雨记录。

GameApp.Garden.cs 管理猜想、亲见确认、修改后的结果失效、物件近景与留卷；GameApp.Revisits.cs 的独立 Board 管理两个旧景小卷；主线进度与回访材料隔离。GardenValidation.cs 枚举有效排列并检查捷径。GameApp.GardenTests.cs 驱动实际 UI，支持 --journey-test。

SaveData 新增 gardenGuess、gardenRead、relics、revisited；默认零值兼容旧旅程，不清空原进度。MusicBed 增加四个 coda，GameApp 增加隔帘滤波和隔水短延迟回应。

默认构建输出 CiJing-v0.15-Windows。旧视频和发布包保留，新版录制素材写入单独 v015 目录；录制工具未作为本轮成片交付验收对象。
