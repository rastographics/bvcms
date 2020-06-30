<template>
    <transition name="slide-left" mode="out-in">
        <div v-if="view == 'phone'" class="text-center" key="phone">
            <div class="well">
                <h3>Enter your cell phone number</h3>
                <p>We will text you a secure one-time code to sign in to your account.</p>
                <div class="row text-left">
                    <div class="col-md-8 col-md-offset-2">
                        <form @submit.prevent="phoneSearch">
                            <div :class="{'form-group': true, 'has-error': showValidation && phone.length < 10}">
                                <input type="tel" class="form-control" v-mask="'(###) ###-####'" v-model="phone" placeholder="(000) 000-0000" autofocus/>
                                <small v-if="showValidation && phone.length < 10" class="text-danger">Please enter your phone number</small>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <button @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </button>
                                </div>
                                <div class="col-md-6">
                                    <input type="submit" class="btn-block btn btn-primary" value="Next" />
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
            <button @click="loadView('email')" class="btn btn-link">Sign in with email address</button>
        </div>
        <div v-if="view == 'email'" class="text-center" key="email">
            <div class="well">
                <h3>Enter your email address</h3>
                <p>We will send you a secure one-time code to sign in to your account.</p>
                <form @submit.prevent="emailSearch">
                    <div class="row text-left">
                        <div class="col-md-6 col-md-offset-3">
                            <div :class="{'form-group': true, 'has-error': showValidation && email.length < 6}">
                                <input type="email" class="form-control" v-model="email" placeholder="you@gmail.com" autofocus />
                                <small v-if="showValidation && email.length < 6" class="text-danger">Please enter your email</small>
                            </div>
                        </div>
                        <div class="col-md-8 col-md-offset-2">
                            <div class="row">
                                <div class="col-md-6">
                                    <button @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </button>
                                </div>
                                <div class="col-md-6">
                                    <input type="submit" class="btn-block btn btn-primary" value="Next" />
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
            <button v-if="SMSReady" @click="loadView('phone')" class="btn btn-link">Sign in with phone number</button>
        </div>
        <div v-if="view == 'code'" class="text-center" key="code">
            <div class="well code-entry">
                <h3>Enter your code</h3>
                <p>Check your messages for a secure one-time code.</p>
                <form @submit.prevent="attemptLogin">
                    <div class="form-inline text-left" style="margin-bottom: 20px;">
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code1" v-model="code1" @paste="pasteCode" @keyup="$refs.code2.focus()" autofocus />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code2" v-model="code2" @paste="pasteCode" @keyup="$refs.code3.focus()" />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code3" v-model="code3" @paste="pasteCode" @keyup="$refs.code4.focus()" />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code4" v-model="code4" @paste="pasteCode" @keyup="$refs.code5.focus()" />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code5" v-model="code5" @paste="pasteCode" @keyup="$refs.code6.focus()" />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code6" v-model="code6" @paste="pasteCode" @keyup="$refs.next.focus()" />
                    </div>
                    <div class="row text-left">
                        <div class="col-md-8 col-md-offset-2">
                            <div class="row">
                                <div class="col-md-6">
                                    <button @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </button>
                                </div>
                                <div class="col-md-6">
                                    <input type="submit" ref="next" class="btn-block btn btn-primary" value="Next" />
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
            <button @click="resendCode" class="btn btn-link">Resend Code</button>
        </div>
        <div v-if="view == 'notfound'" class="text-center" key="notfound">
            <div class="well">
                <h3>We couldn't find you</h3>
                <p v-if="method == 'phone'">Try another phone number, <a href="#" @click="loadView('email')">try an email</a> or <a href="#" @click="loadView('signup')">create an account</a>.</p>
                <p v-else-if="SMSReady">Try another email, <a href="#" @click="loadView('phone')">try a phone number</a> or <a href="#" @click="loadView('signup')">create an account</a>.</p>
                <p v-else>Try another email or <a href="#" @click="loadView('signup')">create an account</a>.</p>
                <div v-if="method == 'phone'" class="row text-left">
                    <div class="col-md-8 col-md-offset-2">
                        <form v-if="method == 'phone'" @submit.prevent="phoneSearch">
                            <div :class="{'form-group': true, 'has-error': showValidation && phone.length < 10}">
                                <input type="tel" class="form-control" v-mask="'(###) ###-####'" v-model="phone" placeholder="(000) 000-0000" autofocus />
                                <small v-if="showValidation && phone.length < 10" class="text-danger">Please enter your phone number</small>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <button @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </button>
                                </div>
                                <div class="col-md-6">
                                    <input type="submit" class="btn-block btn btn-primary" value="Next" />
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
                <form v-else @submit.prevent="emailSearch">
                    <div class="row text-left">
                        <div class="col-md-6 col-md-offset-3">
                            <div :class="{'form-group': true, 'has-error': showValidation && email.length < 6}">
                                <input type="email" class="form-control" v-model="email" placeholder="you@gmail.com" autofocus />
                                <small v-if="showValidation && email.length < 6" class="text-danger">Please enter your email</small>
                            </div>
                        </div>
                        <div class="col-md-8 col-md-offset-2">
                            <div class="row">
                                <div class="col-md-6">
                                    <button @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </button>
                                </div>
                                <div class="col-md-6">
                                    <input type="submit" class="btn-block btn btn-primary" value="Next" />
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
            <button @click="loadView('signup')" class="btn btn-link">Create account</button>
            <button v-if="method == 'phone'" @click="loadView('email')" class="btn btn-link">Sign in with email address</button>
        </div>
    </transition>
</template>
<script>
    import axios from "axios";

    export default {
        props: ["value", "SMSReady"],
        data: function () {
            return {
                view: this.SMSReady ? "phone" : "email",
                showValidation: false,
                method: "",
                phone: "",
                email: "",
                code1: "",
                code2: "",
                code3: "",
                code4: "",
                code5: "",
                code6: ""
            };
        },
        methods: {
            loadView(newView) {
                // setup the new view
                if (['phone', 'email', 'notfound'].includes(newView)) {
                    this.phone = "";
                    this.email = "";
                }
                if (newView == 'phone' && !this.SMSReady) {
                    newView = 'email';
                }
                if (newView == 'code') {
                    this.code1 = "";
                    this.code2 = "";
                    this.code3 = "";
                    this.code4 = "";
                    this.code5 = "";
                    this.code6 = "";
                }
                this.showValidation = false;
                this.view = newView;
            },
            update(value) {
                this.$emit("input", value);
                this.$emit("next");
            },
            phoneSearch() {
                if (this.phone.length < 10) {
                    this.showValidation = true;
                } else {
                    this.method = "phone";
                    this.sendCode(this.phone);
                }
            },
            emailSearch() {
                if (this.email.length < 6) {
                    this.showValidation = true;
                } else {
                    this.method = "email";
                    this.sendCode(this.email);
                }
            },
            pasteCode(ev) {
                var code = (ev.clipboardData || window.clipboardData).getData('text');
                if (code) code = code.trim();
                if ($.isNumeric(code) && code.length == 6) {
                    this.code1 = code[0];
                    this.code2 = code[1];
                    this.code3 = code[2];
                    this.code4 = code[3];
                    this.code5 = code[4];
                    this.code6 = code[5];
                    this.$refs.next.focus();
                }
                return false;   // prevent default
            },
            resendCode() {
                this.sendCode(method == "phone" ? this.phone : this.email);
            },
            sendCode(search) {
                let vm = this;
                axios.post("/Account/SendEasyLoginCode", {
                    search: search
                }).then(
                    response => {
                        if (response.status === 200) {
                            console.log(response.data);
                            if (response.data.Status == "success") {
                                vm.loadView('code');
                            } else {
                                switch (response.data.Message) {
                                    case "No person found":
                                        vm.loadView('notfound');
                                        break;
                                    case "Needs 2FA":
                                        window.location.href = "/Logon?ReturnUrl=" + window.location.pathname + window.location.search
                                    default:
                                        vm.loadView('notfound');
                                        break;
                                }
                            }
                        } else {
                            warning_swal("Warning", "Error");
                            vm.loadView('notfound');
                        }
                    },
                    err => {
                        error_swal("Error", "Error");
                        vm.loadView('notfound');
                    }
                )
                .catch(function (error) {
                    console.log(error);
                });
            },
            attemptLogin() {
                let vm = this;
                let code = "" + vm.code1 + vm.code2 + vm.code3 + vm.code4 + vm.code5 + vm.code6;
                axios.post("/Account/EasyLogin", {
                    search: vm.phone,
                    code: code
                }).then(
                    response => {
                        if (response.status === 200) {
                            console.log(response.data);
                            if (response.data.Status == "success") {
                                vm.$emit('next');
                            } else {
                                switch (response.data.Message) {
                                    case "Invalid code":
                                        alert('Invalid code');
                                        vm.loadView('code');
                                        break;
                                    default:
                                        vm.loadView('notfound');
                                        break;
                                }
                            }
                        } else {
                            warning_swal("Warning", "Error");
                            vm.loadView('notfound');
                        }
                    },
                    err => {
                        error_swal("Error", "Error");
                        vm.loadView('notfound');
                    }
                )
                .catch(function (error) {
                    console.log(error);
                });
            }
        },
        mounted() {
            if (this.value) {
                console.log(this.value);
            }
        }
    }
</script>
<style scoped>
    .code-entry .form-inline .form-control.code-input {
        -webkit-appearance: none;
        -moz-appearance: textfield;
        margin: 0;
        width: 40px;
        padding: 8px;
    }
    .code-input::-webkit-inner-spin-button,
    .code-input::-webkit-outer-spin-button {
        -webkit-appearance: none;
        margin: 0;
    }
    .code-entry .form-inline {
        display: flex;
        justify-content: center;
    }
</style>
