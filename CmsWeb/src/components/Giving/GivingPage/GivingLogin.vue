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
            <button @click="loadView('phone')" class="btn btn-link">Sign in with phone number</button>
        </div>
        <div v-if="view == 'code'" class="text-center" key="code">
            <input ref="code1" @keyup="$refs.code2.focus()" />
            <input ref="code2" @keyup="$refs.code3.focus()" />
            <input ref="code3" @keyup="$refs.code4.focus()" />
            <input ref="code4" @keyup="console.log('hello')" />
        </div>
        <div v-if="view == 'notfound'" class="text-center" key="notfound">
            <div class="well">
                <h3>We couldn't find you</h3>
                <p>Try another phone number, <a href="#" @click="loadView('email')">try an email</a> or <a href="#" @click="loadView('signup')">create an account</a>.</p>
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
            <button @click="loadView('signup')" class="btn btn-link">Create account</button>
            <button @click="loadView('email')" class="btn btn-link">Sign in with email address</button>
        </div>
    </transition>
</template>
<script>
    import axios from "axios";

    export default {
        props: ["value"],
        data: function () {
            return {
                view: "phone",
                phone: "",
                email: "",
                showValidation: false
            };
        },
        methods: {
            loadView(newView) {
                // setup the new view
                if (['phone', 'email', 'notfound'].includes(newView)) {
                    this.phone = "";
                    this.email = "";
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
                    this.sendCode(this.phone);
                }
            },
            emailSearch() {
                if (this.email.length < 6) {
                    this.showValidation = true;
                } else {
                    this.sendCode(this.email);
                }
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
                                    default:
                                        vm.loadView('notfound');
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
                this.$emit("next");
            }
        }
    }
</script>
