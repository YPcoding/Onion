<template>
	<el-dialog title="重置密码" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" :disabled="mode=='show'" ref="dialogForm" label-position="left">
			<el-form-item label="登录密码" prop="password">
					<el-input type="password" v-model="form.password" clearable show-password></el-input>
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible=false" >取 消</el-button>
			<el-button v-if="mode!='show'" type="primary" :loading="isSaveing" @click="submit()" v-auth="'user.resetpassword'">保 存</el-button>
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
					password:"",
				},
			}
		},
		methods: {
			//显示
			open(userIds){
				this.visible = true;
                this.form.userIds = userIds;
				return this
			},
			//表单提交方法
			submit(){
				this.$refs.dialogForm.validate(async (valid) => {
					if (valid) {
						this.isSaveing = true;
						var res = await this.$API.system.user.resetPassword.put(this.form);
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
