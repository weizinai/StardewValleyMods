# 更新日志

# [待定] 0.6.3

- 日志系统迁移至 PiCore（移除 Common 共享项目），控制台日志仍显示各模组名
- Harmony 补丁迁移至 PiCore `Patcher` API（单行 `Patch<T>` 绑定），游戏行为不变
- 配置菜单注册迁移至 PiCore `AddGenericModConfigMenu` builder API（移除模组注册类），游戏行为不变
- 修复"踢出未准备玩家"分区标题误用"显示未准备玩家"标题键的问题