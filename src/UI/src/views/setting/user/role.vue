<template>
	<el-dialog title="分配角色" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" :disabled="mode=='show'" ref="dialogForm" label-position="left">
			<el-form-item prop="roleIds">
				<el-select v-model="form.roleIds" multiple filterable style="width: 100%">
					<el-option v-for="item in groups" :key="item.roleId" :label="item.roleName" :value="item.roleId"/>
				</el-select>
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible=false" >取 消</el-button>
			<el-button v-if="mode!='show'" type="primary" :loading="isSaveing" @click="submit()" v-auth="'user.assigningrole'">保 存</el-button>
		</template>
	</el-dialog>
</template>

<script>
	export default {
		emits: ['success', 'closed'],
		data() {
			return {
				visible: false,
				isSaveing: false,
				//表单数据
				form: {
					userIds:[],
					roleIds:[],
				},
				//所需数据选项
				groups: [],
				groupsProps: {
					value: "id",
					multiple: true,
					checkStrictly: true
				}
			}
		},
		mounted() {
			this.getGroup()
		},
		methods: {
			//显示
			open(userIds){
				this.visible = true;
                this.form.userIds = userIds;
				return this
			},
			//加载树数据
			async getGroup(){			
				var res = await this.$API.system.role.all.get();
				this.groups = res.data;
			},
			//表单提交方法
			submit(){
				this.$refs.dialogForm.validate(async (valid) => {
					if (valid) {
						this.isSaveing = true;
						var res = await this.$API.system.user.assigningRole.put(this.form);
						this.isSaveing = false;
						if(res.succeeded){
							this.visible = false;
							this.$message.success("操作成功")
						}else{
							this.$alert(res.error, "提示", {type: 'error'})
						}
						this.isSaveing = false;					
					}else{
						return false;
					}
				})
			}
		}
	}
</script>

<style>
</style>
