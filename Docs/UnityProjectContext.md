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
