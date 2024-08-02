# 0.4.0

- 添加`ShowMismatchedModInfo`功能，现在主机可以选择是否在SMAPI控制台显示不匹配的模组信息
- 添加`KickPlayer`选项，让玩家可以选择是否踢出不满足模组要求的玩家

# 0.3.0

- 添加`SendSMAPIInfo`选项，让玩家可以选择是否向不满足模组要求的客机玩家发送SMAPI信息
- 添加`add_allow`、`add_require`和`add_ban`命令，使玩家能方便的向当前模组列表中添加模组

# 0.2.2

- 修复命令输入已存在名称时报错的问题

# 0.2.1

- 修复某些情况下导致的空引用问题
- 现在生成模组列表后，`GenericModConfigMenu`会立即刷新
- 现在踢出未安装SMAPI的玩家时，会像该玩家的聊天框发送一条信息
- 现在踢出不满足模组要求的玩家时，会像该玩家的聊天框发送一条信息

# 0.2.0

- 添加`generate_allow`、`generate_require`和`generate_ban`命令，以方便的创建模组列表