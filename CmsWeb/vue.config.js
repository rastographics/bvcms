const path = require('path');

module.exports = {
    outputDir: 'content/dist',
    filenameHashing: false,
    configureWebpack: {
        optimization: {
            splitChunks: false
        },
        resolve: {
            alias: {
                'vue$': 'vue/dist/vue.esm.js',
                'touchpoint': path.resolve(__dirname, './src')
            },
            extensions: ['.js', '.vue', '.json']
        }
    },
}
