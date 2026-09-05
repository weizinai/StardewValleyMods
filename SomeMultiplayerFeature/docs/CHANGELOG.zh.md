# 更新日志

# [待定] 0.20.1

- 日志系统迁移至 PiCore（移除 Common 共享项目），控制台日志仍显示各模组名
- 多人提示消息（Broadcaster）迁移至 PiCore，对端仍按发送方模组名显示
- Harmony 补丁迁移至 PiCore `Patcher` API（单行 `Patch<T>` 绑定），游戏行为不变
- 配置菜单注册迁移至 PiCore `AddGenericModConfigMenu` builder API（移除模组注册类），游戏行为不变