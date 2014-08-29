Vote System 
====
此项目为EM项目组内部投票模块，集成在Team Manager项目中。主要用于投票选出一定时期（暂定每两个月）内的优秀员工及优秀项目，以便于激励员工。

#### 页面规划
----
* 提名页面
    >主要用于开发组长进行提名(即新增VoteItem数据)
    >*Note:* 
    >*1. 需要检查当前时期是否有VoteProject（投票项目），若无则先创建再创建提名，若有则直接创建提名。*
    >*2. 只有各组长才能提名。*
* 投票页面
    > 一般用户投票页面，直接点击Vote按钮即可投票。
    >*Note:*
    >*1. 每个用户投票数有限制（具体数待确定，暂定 **3**），且不能重复投票。*
    >*2. 当前用户已投票项目做特殊标记。*
    >*3. 页面顶端提示（Tips）当前用户已投票数，还可投票数。*
    >*4. 包含New，History按钮用于导航*
* 结果页面
    >显示投票结果，VoteItem得票数倒序排列。
    >*Note:仅在投票时间截止后方可展示。*
* 创建提名页面
    >展示此项目下已有提名Item，并提供创建提名表单。
* History
    >投票结果历史记录

#### 数据表：
----
* 投票主表：VoteProject
        >Id PK
        >name(string) 投票名称
        >TermFrom,TermTo 届别（时间段：2014-08-01~2014-09-30），如此值不为空，则此时间段内只允许一个投票；若此值为空，则是无届别自由投票。 
        >state(int) 状态（255 为删除）
        >BeginTime(datetime) 投票开始时间
        >EndTime(datetime) 投票结束时间
        >createdtime(datetime) 创建时间
        >createdBy(int) 创建人
* 提名主表：VoteItem
        >Id PK
        >PId 外键关联VoteProject
        >name(string) 名称
        >nominees(string) 被提名人Ids
        >nominator(Id) 提名人
        >Comment(string) 提名评语
        >State(int) 状态（255 为删除）
        >CreatedBy 创建人
        >ModifiedBy 修改人
        >CreatedDate 创建时间
        >ModifiedDate 修改时间 
* 投票明细：VoteDetail
        >Id PK
        >PId 外键关联VoteProject
        >IId 外键关联VoteItem
        >Voter(int) 投票人
        >State(int) 状态（0-正常，1-取消，255-删除）
        >CreatedBy 创建人
        >ModifiedBy 修改人
        >CreatedDate 创建时间
        >ModifiedDate 修改时间 

####开发注意事项
* 数据库修改请生成数据库脚本，并放置于：$tfs\Team Mamager\VoteSystem\SqlScripts

####主要技术
----
* MVC + EF6
* BootStrap UI
* Jquery Validate
*  *KnockOutJS（待定）* 

