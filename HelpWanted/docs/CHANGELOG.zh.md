# 更新日志

# [待定] 2.0.5

- 日志系统迁移至 PiCore（移除 Common 共享项目），控制台日志仍显示各模组名
- Harmony 补丁迁移至 PiCore `Patcher` API（单行 `Patch<T>` 绑定），游戏行为不变
- 配置菜单迁移至 PiCore `ConfigService`/描述器配置模块（静态 `ModConfig.Instance`，损坏配置自愈，声明式菜单含原版/RSV 页面、以共享任务子配置分区原语渲染嵌套任务子配置，
  `ExcludeNPCList` 列表字符串选项经逃生舱保持逗号分隔文本编辑），游戏行为不变