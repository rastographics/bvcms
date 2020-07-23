<template>
    <transition name="slide-left" mode="out-in">
        <div v-if="view == 'phone'" class="text-center" key="phone">
            <div class="well">
                <h3>Enter your cell phone number <a href="#" tabindex="0" id="phone_help"><i class="fa fa-question-circle"></i></a></h3>
                <p>We will text you a secure one-time code to sign in to your account.</p>
                <b-popover target="phone_help" placement="bottom" triggers="focus">
                    You can also
                    <a href="#" @click="loadView('email')">sign in with your email</a> or <a href="#" @click="loadView('user')">sign in with a username and password</a>
                </b-popover>
                <div class="row text-left">
                    <div class="col-md-8 col-md-offset-2">
                        <form @submit.prevent="phoneSearch">
                            <div :class="{'form-group': true, 'has-error': showValidation && phone.length < 10}">
                                <input type="tel" class="form-control" v-mask="'(###) ###-####'" v-model="phone" placeholder="(000) 000-0000" autofocus />
                                <small v-if="showValidation" class="text-danger">Please enter your phone number</small>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <a @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </a>
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
                <h3>Enter your email address <a href="#" tabindex="0" id="email_help"><i class="fa fa-question-circle"></i></a></h3>
                <p>We will send you a secure one-time code to sign in to your account.</p>
                <b-popover target="email_help" placement="bottom" triggers="focus">
                    You can also <span v-if="SMSReady"><a href="#" @click="loadView('phone')">sign in with your phone</a> or </span><a href="#" @click="loadView('user')">sign in with a username and password</a>
                </b-popover>
                <form @submit.prevent="emailSearch">
                    <div class="row text-left">
                        <div class="col-md-8 col-md-offset-2">
                            <div :class="{'form-group': true, 'has-error': showValidation && !emailValid}">
                                <input type="email" class="form-control" v-model="email" placeholder="you@gmail.com" autofocus />
                                <small v-if="showValidation" class="text-danger">Please enter your email</small>
                            </div>
                        </div>
                        <div class="col-md-8 col-md-offset-2">
                            <div class="row">
                                <div class="col-md-6">
                                    <a @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </a>
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
        <div v-if="view == 'user'" class="text-center" key="user">
            <div class="well">
                <h3>Sign in with username <a href="#" tabindex="0" id="user_help"><i class="fa fa-question-circle"></i></a></h3>
                <b-popover target="user_help" placement="bottom" triggers="focus">
                    If you don't have a username and password you can also <span v-if="SMSReady"><a href="#" @click="loadView('phone')">sign in with your phone number</a> or </span>
                    <a href="#" @click="loadView('email')">sign in with your email</a>
                </b-popover>
                <form @submit.prevent="userLogin">
                    <div class="row text-left">
                        <div class="col-md-8 col-md-offset-2">
                            <div class="form-group">
                                <input type="text" class="form-control" v-model="username" placeholder="username or email" required autofocus />
                            </div>
                        </div>
                        <div class="col-md-8 col-md-offset-2">
                            <div class="form-group">
                                <input type="password" class="form-control" v-model="password" placeholder="password" required />
                            </div>
                        </div>
                        <div v-if="validationMsg" class="col-sm-12 text-center">
                            <p class="text-danger">{{ validationMsg }}</p>
                        </div>
                        <div class="col-md-8 col-md-offset-2">
                            <div class="row">
                                <div class="col-md-6">
                                    <a @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </a>
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
                <form @submit.prevent="easyLogin">
                    <div class="form-inline text-left" style="margin-bottom: 20px;">
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code1" v-model="code1" @paste="pasteCode" @input="$refs.code2.focus()" autofocus />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code2" v-model="code2" @paste="pasteCode" @input="$refs.code3.focus()" />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code3" v-model="code3" @paste="pasteCode" @input="$refs.code4.focus()" />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code4" v-model="code4" @paste="pasteCode" @input="$refs.code5.focus()" />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code5" v-model="code5" @paste="pasteCode" @input="$refs.code6.focus()" />
                        <input type="number" min="0" max="9" class="form-control code-input" ref="code6" v-model="code6" @paste="pasteCode" @input="$refs.next.focus()" />
                    </div>
                    <div v-if="validationMsg" class="text-center">
                        <p class="text-danger">{{ validationMsg }}</p>
                    </div>
                    <div class="row text-left">
                        <div class="col-md-8 col-md-offset-2">
                            <div class="row">
                                <div class="col-md-6">
                                    <a @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                                        Back
                                    </a>
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
        <div v-if="view == 'userselect'" class="text-center" key="notfound">
            <h3>Select person <a href="#" tabindex="0" id="person_select"><i class="fa fa-question-circle"></i></a></h3>
            <b-popover target="person_select" placement="bottom" triggers="focus">
                Why am I seeing multiple people?
            </b-popover>
            <p>Who is making this gift?</p>
            <div class="row" v-for="user in userResults" :key="user.PeopleId">
                <div class="col-sm-6 col-sm-offset-3">
                    <a @click="sendCodeTo(user.PeopleId)" class="btn btn-block btn-default" style="margin-bottom: 14px;">{{ user.Name }}</a>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-6 col-sm-offset-3">
                    <button @click="$emit('back')" class="btn-block btn btn-default" tabindex="-1">
                        Back
                    </button>
                </div>
            </div>
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
                                <small v-if="showValidation" class="text-danger">Please enter your phone number</small>
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
                            <div :class="{'form-group': true, 'has-error': showValidation && !emailValid}">
                                <input type="email" class="form-control" v-model="email" placeholder="you@gmail.com" autofocus />
                                <small v-if="showValidation" class="text-danger">Please enter your email</small>
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
        <div v-if="view == 'signup'" class="text-center" key="signup">
            <div class="well">
                <h3>Create Account</h3>
                <p>We couldn't find your account. No problem! Let's create one:</p>
                <form @submit.prevent="register">
                    <div class="row text-left">
                        <div class="col-md-6">
                            <div class="form-group">
                                <input type="text" class="form-control" v-model="newUser.first" placeholder="First Name" autofocus required />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <input type="text" class="form-control" v-model="newUser.last" placeholder="Last Name" required />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div :class="{'form-group': true, 'has-error': showValidation && !newUserEmailValid}">
                                <input type="email" class="form-control" v-model="newUser.email" placeholder="Email Address" />
                                <small v-if="showValidation" class="text-danger">Please enter your email</small>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div :class="{'form-group': true, 'has-error': showValidation && newUser.phone.length < 10}">
                                <input type="tel" class="form-control" v-model="newUser.phone" placeholder="Phone Number" />
                                <small v-if="showValidation" class="text-danger">Please enter your phone</small>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group">
                                <input type="text" class="form-control" v-model="newUser.address" placeholder="Address" required />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <input type="text" class="form-control" v-model="newUser.city" placeholder="City" required />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <input type="text" class="form-control" v-model="newUser.state" placeholder="State" required />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <input type="text" class="form-control" v-model="newUser.zip" placeholder="Zip" required />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <input type="text" class="form-control" v-model="newUser.country" placeholder="Country" required />
                            </div>
                        </div>
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
        <div v-if="view == '2fa'" class="text-center" key="2fa">
            <div class="well">
                <h3>Welcome back</h3>
                <p>Because your account has 2 factor authentication enabled, you'll need to sign in with your username and password.</p>
                <div class="row">
                    <div class="col-sm-6 col-sm-offset-3">
                        <a :href="loginUrl" class="btn btn-block btn-primary">Sign In</a>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>
<script>
    import axios from "axios";
    import { utils } from "touchpoint/common/utils.js";

    export default {
        props: ["value", "SMSReady", "loginUrl"],
        data() {
            return {
                view: "",
                showValidation: false,
                validationMsg: "",
                method: "",
                phone: "",
                email: "",
                username: "",
                password: "",
                code1: "",
                code2: "",
                code3: "",
                code4: "",
                code5: "",
                code6: "",
                loginas: null,
                userResults: [],
                newUser: {
                    first: "",
                    last: "",
                    email: "",
                    phone: "",
                    address: "",
                    city: "",
                    state: "",
                    zip: "",
                    country: ""
                }
            };
        },
        computed: {
            emailValid: function () {
                return utils.validateEmail(this.email);
            },
            newUserEmailValid: function () {
                return utils.validateEmail(this.newUser.email);
            }
        },
        watch: {
            view: function () {
                setTimeout(function () {
                    $('[autofocus]').focus();
                }, 1000);
            }
        },
        methods: {
            loadView(newView) {
                let vm = this;
                // setup the new view
                if (['phone', 'email', 'user', 'notfound'].includes(newView)) {
                    vm.phone = "";
                    vm.email = "";
                    vm.username = "";
                    vm.password = "";
                }
                if (newView == 'phone' && !vm.SMSReady) {
                    newView = 'email';
                }
                if (newView == 'code') {
                    vm.code1 = "";
                    vm.code2 = "";
                    vm.code3 = "";
                    vm.code4 = "";
                    vm.code5 = "";
                    vm.code6 = "";
                }
                vm.showValidation = false;
                vm.view = newView;
                return false;
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
                if (!this.emailValid) {
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
                this.sendCode(this.method == "phone" ? this.phone : this.email, true);
            },
            sendCodeTo(pid) {
                this.sendCode(this.method == "phone" ? this.phone : this.email, false, pid);
            },
            sendCode(search, resend, sendto = null) {
                let vm = this;
                vm.loginas = sendto;
                axios.post("/Account/SendEasyLoginCode", {
                    search: search,
                    sendto: sendto
                }).then(
                    response => {
                        if (response.status === 200) {
                            if (response.data.Status == "success") {
                                if (response.data.Message.startsWith('[{')) {
                                    // multiple results
                                    var result = JSON.parse(response.data.Message);
                                    vm.userResults = result;
                                    vm.loadView('userselect');
                                } else {
                                    vm.loadView('code');
                                }
                                if (resend) {
                                    vm.validationMsg = "Code resent";
                                }
                            } else {
                                switch (response.data.Message) {
                                    case "No person found":
                                        vm.loadView('notfound');
                                        break;
                                    case "Needs 2FA":
                                        vm.loadView('2fa');
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
            },
            easyLogin() {
                let vm = this;
                let code = "" + vm.code1 + vm.code2 + vm.code3 + vm.code4 + vm.code5 + vm.code6;
                if ($.isNumeric(code) && code.length == 6) {
                    axios.post("/Account/EasyLogin", {
                        search: vm.method == 'phone' ? vm.phone : vm.email,
                        code: code,
                        loginas: vm.loginas
                    }).then(
                        response => {
                            if (response.status === 200) {
                                if (response.data.Status == "success") {
                                    vm.$emit('next');
                                } else {
                                    switch (response.data.Message) {
                                        case "Invalid code":
                                            vm.validationMsg = "Invalid code. Please enter the code from the most recent message we sent.";
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
                } else {
                    vm.validationMsg = "Please enter the 6 digit code in the message we sent."
                    vm.loadView('code');
                }
            },
            userLogin() {
                let vm = this;
                axios.post("/Account/SigninWithUsername", {
                    usernameOrEmail: vm.username,
                    password: vm.password
                }).then(
                    response => {
                        if (response.status === 200) {
                            if (response.data.Status == "success") {
                                vm.$emit('next');
                            } else {
                                switch (response.data.Message) {
                                    case "Needs 2FA":
                                        vm.loadView('2fa');
                                        break;
                                    default:
                                        vm.validationMsg = response.data.Message;
                                        vm.loadView('user');
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
            register() {

            }
        },
        mounted() {
            this.view = this.SMSReady ? "phone" : "email";
        }
    }
</script>
<style scoped>
    a {
        cursor: pointer;
    }
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
