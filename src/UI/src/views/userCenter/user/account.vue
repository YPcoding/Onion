<template>
	<el-card shadow="never" header="个人信息">
		<el-form ref="form" :model="form" label-width="120px" style="margin-top:20px;">
			<el-form-item label="账号">
				<el-input v-model="form.userName" disabled></el-input>
				<div class="el-form-item-msg">账号信息用于登录，系统不允许修改</div>
			</el-form-item>
			<el-form-item label="姓名">
				<el-input v-model="form.realname"></el-input>
			</el-form-item>
			<el-form-item label="昵称">
				<el-input v-model="form.nickname"></el-input>
			</el-form-item>
			<el-form-item label="性别">
				<el-select v-model="form.genderType" placeholder="请选择">
					<el-option label="保密" value="Secrecy"></el-option>
					<el-option label="男" value="Male"></el-option>
					<el-option label="女" value="Female"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item label="生日" prop="birthday">
					<el-date-picker type="birthday" placeholder="选择生日" value-format="YYYY-MM-DD" v-model="form.birthday"></el-date-picker>
				</el-form-item>
			<el-form-item label="个性签名">
				<el-input v-model="form.signature" type="textarea"></el-input>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="save" :loading="isSaving">保存</el-button>
			</el-form-item>
		</el-form>
	</el-card>
</template>

<script>
	export default {
		data() {
			return {
				isSaving:false,
				form: {
					concurrencyStamp:"",
					userName: "",
					realname: "",
					nickname:"",
					genderType: null,
					birthday:"",
					signature: ""
				}
			}
		},
		async mounted() {
			this.search();
		},
		methods: {
			async search(){
				var user = await this.$API.system.user.info.get();
			    if (user.succeeded) {
				    this.form = user.data;
			    }
			},
			async save(){
				this.isSaving = true;
				var response = await this.$API.system.user.updateInfo.put(this.form);
			    if (response?.succeeded) {
					this.$message.success("修改成功")
					this.search();
			    }
				this.isSaving =false;
			}
		}
	}
</script>

<style>
</style>
