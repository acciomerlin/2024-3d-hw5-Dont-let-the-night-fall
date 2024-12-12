# 2024-3d-hw5-Dont-let-the-night-fall
# 游戏设计与实现博客：3D弩箭射击游戏

## 游戏简介

本项目是一款基于Unity的3D弩箭射击游戏，玩家需要在时间循环的场景中射击固定靶和运动靶，完成目标数量以阻止夜晚降临。游戏融合了第一人称视角、物理效果、动画，以及多样化的音效和视觉效果，提供沉浸式的射击体验。

---

## 游戏场景设计

### 地形（2分）

- 使用Unity地形组件创建了带山丘、道路、草地和树木的自然场景。
- 地形经过细节调节，并结合第三方资源优化了视觉效果。

### 天空盒（2分）

- 实现了三种天空盒（白天、黄昏、夜晚），并根据时间自动切换。
- 天空盒状态影响靶标生成、追逐者速度、音乐和玩家目标。

### 固定靶（2分）

- 在场景中布置了静态的靶标，玩家可以使用弩箭射击。
- 靶标被射中后，生成嵌入箭矢，触发粒子效果。

### 运动靶（2分）

- 运动靶标使用动画控制轨迹和速度，在射中后自然坠落。
- 运动轨迹随机，增加挑战性。

### 射击位（2分）

- 地图上设定了若干射击位，玩家只能在射击位附近拉弓射箭。
- 每个射击位限定次数，玩家需合理分配射击机会。

### 摄像机（2分）

- 多摄像机视角设计：第一人称视角用于瞄准；鸟瞰图显示整体场景，方便玩家战略规划。
- 瞄准状态下切换至精准视角，模拟弩箭瞄准镜效果。

### 声音（2分）

- 背景音乐随时间状态（白天、黄昏、夜晚）切换。
- 射击、命中、追逐者接近等场景配有特定音效，增强沉浸感。

---

## 运动与物理与动画

### 游走（2分）

- 使用第一人称控制器实现自由移动，玩家可探索地图，避免与障碍物（树木、靶标等）碰撞。

### 射击效果（2分）

- 使用物理引擎模拟弩箭飞行轨迹。
- 命中靶标后生成嵌入箭矢，同时触发粒子和声音效果。

### 碰撞与计分（2分）

- 碰撞检测与计分系统管理游戏规则，命中靶标增加分数。
- 支持现场修改规则，例如调整目标数或靶标生成频率。

### 弩弓动画（2分）

- 动画融合弩弓的蓄力、瞄准和射击动作。
- 滚轮调整射击力，并在UI上实时显示。

---

## 游戏与创新

### 创意场景与道具

- 追逐者作为动态威胁，夜晚状态下逐步加速，增加紧张感。
- 玩家必须射击靶标完成目标数，以返回白天。

### 特效与音效

- 追逐者靠近时音效音量递增，动态模拟威胁感。
- 瞄准、射箭、命中分别触发对应的音效。

### 力场与AI

- 追逐者通过AI导航追踪玩家位置，结合动态力场实现逼真的运动行为。

### 玩法优化

- 玩家需在有限时间内射击目标，避免追逐者抓捕。
- 完成目标后恢复白天，通过游戏循环增加难度。
