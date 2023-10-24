'use strict';

(function (angular, factory) {
    if (typeof define === 'function' && define.amd) {
        define(['angular', 'ckeditor'], function (angular) {
            return factory(angular);
        });
    } else {
        return factory(angular);
    }
}(angular || null, function (angular) {
    var app = angular.module('ngCkeditor', []);
    var $defer, loaded = false;

    app.run(['$q', '$timeout', function ($q, $timeout) {
        $defer = $q.defer();

        if (angular.isUndefined(CKEDITOR)) {
            throw new Error('CKEDITOR not found');
        }
        CKEDITOR.disableAutoInline = true;
        function checkLoaded() {
            if (CKEDITOR.status === 'loaded') {
                loaded = true;
                $defer.resolve();
            } else {
                checkLoaded();
            }
        }

        CKEDITOR.on('loaded', checkLoaded);
        $timeout(checkLoaded, 100);
    }]);

    app.directive('ckeditor', ['$timeout', '$q', function ($timeout, $q) {

        return {
            restrict: 'AC',
            require: ['ngModel', '^?form'],
            scope: false,
            link: function (scope, element, attrs, ctrls) {
                var ngModel = ctrls[0];
                var form = ctrls[1] || null;
                var EMPTY_HTML = '<p></p>',
                    isTextarea = element[0].tagName.toLowerCase() === 'textarea',
                    data = [],
                    isReady = false;

                if (!isTextarea) {
                    element.attr('contenteditable', true);
                }

                var onLoad = function () {

                    var options = {
                        //toolbar: 'full',
                        //toolbar_full: [ //jshint ignore:line
                        //    {
                        //        name: 'basicstyles',
                        //        items: ['Bold', 'Italic', 'Strike', 'Underline']
                        //    },
                        //    {name: 'paragraph', items: ['BulletedList', 'NumberedList', 'Blockquote']},
                        //    {name: 'editing', items: ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock']},
                        //    {name: 'links', items: ['Link', 'Unlink', 'Anchor']},
                        //    {name: 'tools', items: ['SpellChecker', 'Maximize']},
                        //    '/',
                        //    {
                        //        name: 'styles',
                        //        items: ['Format', 'FontSize', 'TextColor', 'PasteText', 'PasteFromWord', 'RemoveFormat']
                        //    },
                        //    {name: 'insert', items: ['Image', 'Table', 'SpecialChar']},
                        //    {name: 'forms', items: ['Outdent', 'Indent']},
                        //    {name: 'clipboard', items: ['Undo', 'Redo']},
                        //    {name: 'document', items: ['PageBreak', 'Source']}
                        //],
                        disableNativeSpellChecker: false,
                        uiColor: '#FAFAFA',
                        height: '400px',
                        width: '100%'
                    };

                    //options = angular.extend(options, scope[attrs.ckeditor]);

                    var roxyFileman = '/fileman/index.html';

                    options = angular.extend(options, {
                        language: 'pt-br',
                        filebrowserBrowseUrl: roxyFileman,
                        filebrowserImageBrowseUrl: roxyFileman + '?type=image',
                        removeDialogTabs: 'link:upload;image:upload'
                    });

                    var instance = (isTextarea) ? CKEDITOR.replace(element[0], options) : CKEDITOR.inline(element[0], options),
                        configLoaderDef = $q.defer();

                    element.bind('$destroy', function () {
                        if (instance && CKEDITOR.instances[instance.name]) {
                            CKEDITOR.instances[instance.name].destroy();
                        }
                    });
                    var setModelData = function (setPristine) {
                        var data = instance.getData();
                        if (data === '') {
                            data = null;
                        }
                        $timeout(function () { // for key up event
                            if (setPristine !== true || data !== ngModel.$viewValue) {
                                ngModel.$setViewValue(data);
                            }

                            if (setPristine === true && form) {
                                form.$setPristine();
                            }
                        }, 0);
                    }, onUpdateModelData = function (setPristine) {
                        if (!data.length) {
                            return;
                        }

                        var item = data.pop() || EMPTY_HTML;
                        isReady = false;
                        instance.setData(item, function () {
                            setModelData(setPristine);
                            isReady = true;
                        });
                    };

                    instance.on('pasteState', setModelData);
                    instance.on('change', setModelData);
                    instance.on('blur', setModelData);
                    //instance.on('key',          setModelData); // for source view

                    instance.on('instanceReady', function () {
                        scope.$broadcast('ckeditor.ready');
                        scope.$apply(function () {
                            onUpdateModelData(true);
                        });

                        instance.document.on('keyup', setModelData);
                    });
                    instance.on('customConfigLoaded', function () {
                        configLoaderDef.resolve();
                    });

                    ngModel.$render = function () {
                        data.push(ngModel.$viewValue);
                        if (isReady) {
                            onUpdateModelData();
                        }
                    };

                    /* BOTOES PERSONALIZADOS */
                    var editor = {};

                    for (var item in CKEDITOR.instances) {
                        editor = CKEDITOR.instances[item];
                    }

                    editor.addCommand('cmdNomeCurso', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#TITULO_CURSO]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('nome-curso', {
                        label: 'T&Iacute;TULO_DO_CURSO',
                        command: 'cmdNomeCurso',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdNomeCursista', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#NOME_CURSISTA]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('nome-cursista', {
                        label: 'NOME_DO_CURSISTA',
                        command: 'cmdNomeCursista',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdDataInicial', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#DATA_INICIAL]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('data-inicial', {
                        label: 'DATA_INICIAL',
                        command: 'cmdDataInicial',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdDataFinal', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#DATA_FINAL]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('data-final', {
                        label: 'DATA_FINAL',
                        command: 'cmdDataFinal',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdCargaHoraria', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#CARGA_HORARIA]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('carga-horaria', {
                        label: 'CARGA_HOR&Aacute;RIA',
                        command: 'cmdCargaHoraria',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdConteudoProgramatico', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#CONTEUDO_PROGRAMATICO]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('conteudo-programatico', {
                        label: 'CONTE&Uacute;DO_PROGRAM&Aacute;TICO',
                        command: 'cmdConteudoProgramatico',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdNomeDocente', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#NOME_DOCENTE]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('nome-docente', {
                        label: 'NOME_DO_DOCENTE',
                        command: 'cmdNomeDocente',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdLocal', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#LOCAL]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('local', {
                        label: 'LOCAL',
                        command: 'cmdLocal',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdDataAtual', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#DATA_ATUAL]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('data-atual', {
                        label: 'DATA_ATUAL',
                        command: 'cmdDataAtual',
                        toolbar: 'eventos'
                    });

                    editor.addCommand('cmdNumeroAutenticacao', {
                        exec: function (editor) {
                            var $form = editor.element.$.form;
                            if ($form) {
                                try {
                                    editor.updateElement();
                                    editor.insertHtml('<span>[#NUMERO_AUTENTICACAO]</span>');
                                }
                                catch (e) {
                                    alert(e);
                                }
                            }
                        }
                    });

                    editor.ui.addButton('numero-autenticacao', {
                        label: 'NUMERO_AUTENTICACAO',
                        command: 'cmdNumeroAutenticacao',
                        toolbar: 'eventos'
                    });

                    CKEDITOR.config.toolbar = [ //jshint ignore:line
                        { name: 'basicstyles', items: ['Bold', 'Italic', 'Strike', 'Underline'] },
                        { name: 'paragraph', items: ['BulletedList', 'NumberedList', 'Blockquote'] },
                        { name: 'editing', items: ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
                        { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
                        { name: 'tools', items: ['SpellChecker', 'Maximize'] },
                        '/',
                        { name: 'styles', items: ['Format', 'FontSize', 'TextColor', 'PasteText', 'PasteFromWord', 'RemoveFormat'] },
                        { name: 'insert', items: ['Image', 'Table', 'SpecialChar'] },
                        { name: 'forms', items: ['Outdent', 'Indent'] },
                        { name: 'clipboard', items: ['Undo', 'Redo'] },
                        { name: 'document', items: ['PageBreak', 'Source'] },
                        '/',
                        { name: 'eventos', items: ['nome-curso', 'nome-cursista', 'nome-docente', 'local', 'data-atual', 'data-inicial', 'data-final', 'carga-horaria', 'conteudo-programatico', 'codigo-autenticacao'] }
                    ];

                    CKEDITOR.config.allowedContent = true;
                };

                if (CKEDITOR.status === 'loaded') {
                    loaded = true;
                }
                if (loaded) {
                    onLoad();
                } else {
                    $defer.promise.then(onLoad);
                }
            }
        };
    }]);

    return app;
}));