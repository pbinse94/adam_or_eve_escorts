const path = require('path');
const webpack = require('webpack');

module.exports = {
    entry: './Scripts/index.js', // Entry point of your application
    output: {
        path: path.resolve(__dirname, 'wwwroot'),
        filename: 'bundle.js' // Output file name
    },
    mode: 'production',
    module: {
        rules: [
            {
                test: /\.js$/, // Transpile all .js files
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env']
                    }
                }
            }
        ]
    }
};