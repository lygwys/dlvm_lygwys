require.config({
    urlArgs: "v=" + (typeof DLVM_PAGER_VERSION === "string" ? DLVM_PAGER_VERSION : ''),
    baseUrl: '/content/js/',
    waitSeconds: 1000,
    paths: {//配置模块的引用路径，可以本地相对路径，也可以是外部路径，可以提供多个路径
        //lib
        'jquery': 'jquery.min',
        'bootstrap': 'bootstrap.min',
        'jqueryBootstrap': 'jquery.bootstrap.min',
        'mousewheel': 'jquery.mousewheel',
        'jquery-mousewheel': 'jquery.mousewheel',
        //jquery-ui
        'jquery.ui': 'jquery-ui-1.10.3/ui/jquery.ui.core',
        'jquery.ui.widget': 'jquery-ui-1.10.3/ui/jquery.ui.widget',
        'jquery.ui.mouse': 'jquery-ui-1.10.3/ui/jquery.ui.mouse',
        'jquery.ui.draggable': 'jquery-ui-1.10.3/ui/jquery.ui.draggable',

        //mzg
        'mzg': 'mzg',
        'mzg.jquery': 'mzg.jquery',
        'mzg.web': 'mzg.web',
        'fetch': 'fetch',
        'mzgmain.plugs': 'mzgmain.plugs',
        'mzg-history': 'mzg-history',
        'createForm': 'form',
        'renderForm': 'renderform',
        'calculation': 'calculation',

        //jquery-plugs
        'jquery.toast': 'jquery-toast/jquery.toast.min',
        'jquery.tmpl': 'jquery.tmpl',
        'jquery.cookie': 'jquery.cookie',
        'jquery.bootpag': 'jquery.bootpag.min',
        'jquery.form': 'jquery.form',
        'jquery.tableresize': 'jquery.tableresize',
        'bootstrap-datepicker': 'bootstrap-datepicker-1.5.0/js/bootstrap-datepicker.min',
        'bootstrap-datepicker-cn': 'bootstrap-datepicker-1.5.0/locales/bootstrap-datepicker.zh-CN.min',

        'jquery-validate': 'jquery-validate/jquery.validate.min',
        'jquery-validate-zh': 'jquery-validate/localization/messages_zh.min',
        'jquery.dirtyforms': 'jquery.dirtyforms',
        'jquery.printTable': 'jquery.printTable',
        'jquery.datetimepicker.full': 'bootstrap-datetimepicker/jquery.datetimepicker.full'

        //uediter
        , 'ueditor': 'ueditor/ueditor.all.min'
        , 'ueditor.config': 'ueditor/ueditor.config'
        , 'ueditor.addcustomizebutton': 'ueditor/addcustomizebutton'

        //common-modules
        , 'notice': 'common/notice'
        , 'filters': 'common/filters'
        , 'navtree': 'common/navtree'
        , 'charts': 'common/charts'
        , 'formSearcher': 'common/formsearcher'
        , 'selectEntityDialog': 'common/selectentityDialog'
        , 'dirtychecker': 'common/dirtychecker'
        , 'formular': 'common/formular'

        //pages
        , 'home.index': 'pages/home.index'
        , 'entity.list': 'pages/entity.list'
        , 'entity.gridview': 'pages/entity.gridview'
        , 'entity.create': 'pages/entity.create'
    },
    map: {
        '*': {
            'css': './requirejs/css.min' // or whatever the path to require-css is
        }
    },
    //依赖用于配置在脚本/模块外面并没有使用RequireJS的函数依赖并且初始化函数。假设underscore并没有使用 RequireJS定义，但是你还是想通过RequireJS来使用它，那么你就需要在配置中把它定义为一个shim。
    shim: {
        //'bootstrap': {
        //    deps: ['css!../css/a.css']
        //}
        'bootstrap': {
            deps: ['jquery']
        },
        'bootstrap': {
            deps: ['jquery', 'css!/content/css/bootstrap.min.css']
        },
        'jqueryBootstrap': {
            deps: ['jquery', 'bootstrap']
        },
        //jquery-ui
        'jquery.ui': {
            deps: ['jquery']
        },
        'jquery.ui.widget': {
            deps: ['jquery.ui']
        },
        'jquery.ui.mouse': {
            deps: ['jquery.ui.widget']
        },
        'jquery.ui.draggable': {
            deps: ['jquery.ui.mouse', 'jquery.ui.widget']
        },
        //jquery-plugs
        'jquery.toast': {
            deps: ['jquery']
        },
        'jquery.tmpl': {
            deps: ['jquery']
        },
        'jquery.cookie': {
            deps: ['jquery']
        },
        'jquery.bootpag': {
            deps: ['jquery']
        },
        'jquery.form': {
            deps: ['jquery']
        },
        'jquery-validate': {
            deps: ['jquery']
        },
        'jquery.printTable': {
            deps: ['jquery']
        },
        'jquery.tableresize': {
            deps: ['jquery']
        },
        'jquery.dirtyforms': {
            deps: ['jquery']
        },
        'bootstrap-datepicker': {
            deps: ['bootstrap']
        },
        'bootstrap-datepicker-cn': {
            deps: ['jquery', 'bootstrap-datepicker']
        },
        //'jquery.datetimepicker.full': {
        //    deps: ['jquery','jquery-mousewheel']
        //},

        //'mousewheel': {
        //    deps: ['jquery.datetimepicker.full']
        //},
        //mzg

        'mzg': {
            deps: ['jquery']
        },
        'mzg.jquery': {
            deps: ['mzg']
        },
        'mzg.web': {
            deps: ['mzg', 'mzg.jquery', 'jquery.ui.draggable']
        },
        'mzgmain.plugs': {
            deps: ['jquery', 'jquery.tmpl']
        },
        'mzg-history': {
            deps: ['jquery']
        },
        'fetch': {
            deps: ['mzg']
        },
        'createForm': {
            deps: ['mzg']
        },
        'renderForm': {
            deps: ['mzg']
        },
        'calculation': {
            deps: ['mzg']
        },
        //common
        'filters': {
            deps: ['fetch']
        },
        'charts': {
            deps: ['formSearcher']
        },
        //pages
        'entity.list': {
            deps: ['filters', 'mzg.web', 'entity.gridview']
        },
        'entity.gridview': {
            deps: ['charts', 'formSearcher', 'mzg.jquery', 'mzg.web']
        },
        'entity.create': {
            deps: ['formular', 'dirtychecker', 'mzg.jquery', 'mzg.web', 'createForm', 'renderForm', 'calculation',]
        },
        'home.index': {
            deps: ['jquery']
        }
    },
});