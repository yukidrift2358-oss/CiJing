from pathlib import Path
import sys
B=Path(__file__).resolve().parent.parent;W=B.parents[1]
from diagram_canvas import Canvas,INK,RED,MUTE
c=Canvas();c.text(960,160,'合成改变起音次数与占位',48)
for x in [350,650]:c.circle(x,420,90,'宫 · 短')
c.text(500,420,'＋',50,MUTE);c.arrow([(820,420),(1000,420)]);c.circle(1200,420,90,'宫 · 长');c.circle(1510,420,90,'空位',False)
c.text(500,660,'材料 2 份 · 占 2 格');c.text(1350,660,'材料 2 份 · 占 1 格')
c.arrow([(1050,780),(780,780)],MUTE);c.text(960,835,'需要两次起音时，可以拆分',34,MUTE);c.save('resource')
c=Canvas();c.text(960,170,'第二章：演奏改变景物状态',48)
for x,t in [(360,'雨声'),(960,'留白'),(1560,'曲尾')]:c.circle(x,430,115,t)
c.arrow([(500,430),(810,430)]);c.arrow([(1100,430),(1410,430)])
c.text(360,660,'每滴雨分别听清',34);c.text(960,660,'风退、水波消散',34);c.text(1560,660,'检查当时景物',34)
c.text(960,850,'有效目标不只一种摆法；材料和曲尾状态仍有限制。',34,MUTE);c.save('goals')
c=Canvas();c.text(960,150,'同样四份材料，两种卷帘结果',48)
c.text(960,280,'两滴短雨分开，至少一次在帘起前听见',34,MUTE)
for y,wind,outcome in [(450,'两份羽合成一次长风','花落 · 露出旧笺'),(690,'两份羽保留两次短风','花稀 · 留一枝残红')]:
 c.text(500,y,wind,36);c.arrow([(860,y),(1080,y)]);c.text(1390,y,outcome,36)
c.text(960,900,'曲尾帘起、风歇、水定 → 观花比照 → 确认入卷',36,RED);c.save('routes')
c=Canvas(False);c.text(960,200,'词境 v0.15',65);c.text(960,360,'试玩 · 源码 · 版本比较 · 完整演示',40)
c.text(960,530,'https://github.com/yukidrift2358-oss/CiJing',34,RED)
c.text(960,650,'https://github.com/yukidrift2358-oss/CiJing/releases/tag/v0.15',30,RED)
c.text(960,850,'讲解稿、字幕、章节与制作记录随展示文件保留',30,MUTE);c.save('endcard')
print('V3_DIAGRAMS_READY')
