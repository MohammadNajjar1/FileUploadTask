const fs = require("fs");
const glob = require("glob");
const path = require("path");
const esbuild = require("esbuild");
const started = process.hrtime();

glob.sync("./wwwroot/js/**/*.min.js").forEach(fs.unlinkSync);
glob.sync("./wwwroot/css/**/*.min.css").forEach(fs.unlinkSync);

const vendorJs = bundle([
    "./wwwroot/js/moment/moment.js",
    "./wwwroot/js/moment/**/*.js",
    "./wwwroot/js/popper/*.js",
    "./wwwroot/js/rome/*.js",
    "./wwwroot/js/mvc-lookup/mvc-lookup.js",
    "./wwwroot/js/mvc-lookup/**/*.js",
    "./wwwroot/js/mvc-grid/mvc-grid.js",
    "./wwwroot/js/mvc-grid/**/*.js",
    "./wwwroot/js/mvc-tree/*.js",
    "./wwwroot/js/bootstrap/*.js",
    "./wwwroot/js/wellidate/*.js",
    "./wwwroot/js/shared/widgets/*.js"
], "./wwwroot/js/site/vendor.min.js");

const siteJs = bundle([
    "./wwwroot/js/shared/site.js"
], "./wwwroot/js/site/bundle.min.js");

const appJs = minify(["./wwwroot/js/application/**/*.js"]);

const vendorCss = bundle([
    "./wwwroot/css/rome/*.css",
    "./wwwroot/css/bootstrap/*.css",
    "./wwwroot/css/font-awesome/*.css",
    "./wwwroot/css/mvc-grid/*.css",
    "./wwwroot/css/mvc-tree/*.css",
    "./wwwroot/css/mvc-lookup/*.css"
], "./wwwroot/css/site/vendor.min.css");

const siteCss = bundle([
    "./wwwroot/css/shared/content.css",
    "./wwwroot/css/shared/header.css",
    "./wwwroot/css/shared/navigation.css",
    "./wwwroot/css/shared/overrides.css",
    "./wwwroot/css/shared/table.css",
    "./wwwroot/css/shared/widget-box.css"
], "./wwwroot/css/site/bundle.min.css");

const appCss = minify(["./wwwroot/css/application/**/*.css"]);

Promise.all([vendorJs, siteJs, ...appJs, vendorCss, siteCss, ...appCss]).then(_ => {
    const ended = process.hrtime(started);

    console.log("Bundled in: \x1b[32m%ds %dms\x1b[0m", ended[0], ended[1] / 1000000);
});

function bundle(files, outFile) {
    return esbuild.build({
        entryPoints: [...new Set(files.map(pattern => glob.sync(pattern)).flat())],
        outdir: "./tmp",
        minify: true,
        write: false
    }).then(result => {
        const dir = path.dirname(outFile);

        if (!fs.existsSync(dir)) {
            fs.mkdirSync(dir);
        }

        const bundle = fs.openSync(outFile, "w");

        result.outputFiles.forEach(file => {
            fs.writeSync(bundle, file.text);
        });
    });
}

function minify(files) {
    return [...new Set(files.map(pattern => glob.sync(pattern)).flat())].map(file => esbuild.build({
        entryPoints: [file],
        outExtension: { ".js": ".min.js", ".css": ".min.css" },
        outdir: path.dirname(file),
        minify: true
    }));
}
