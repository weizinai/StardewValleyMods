# 更新日志

# [待定] 0.6.3

- 日志系统迁移至 PiCore（移除 Common 共享项目），控制台日志仍显示各模组名
- Harmony 补丁迁移至 PiCore `Patcher` API（单行 `Patch<T>` 绑定），游戏行为不变
- 配置菜单迁移至 PiCore `ConfigService`/描述器配置模块（静态 `ModConfig.Instance`，损坏配置自愈），游戏行为不变
- 修复"踢出未准备玩家"分区标题误用"显示未准备玩家"标题键的问题