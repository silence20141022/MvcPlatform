﻿function form_ini() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;基础信息</span></h4>'
    }

    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo]
    })
    //订单编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        id: 'field_CODE',
        name: 'CODE',
        fieldLabel: '订单编号',
        readOnly: true,
        emptyText: '订单号自动生成'
    });
    //委托类型
    var store_ENTRUSTTYPENAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: wtlx_js_data
    })
    var combo_ENTRUSTTYPENAME = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPENAME',
        name: 'ENTRUSTTYPEID',
        hideTrigger: true,
        store: store_ENTRUSTTYPENAME,
        fieldLabel: '委托类型',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 2,
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            },
            select: function (combo, records, eOpts) {
                wtlx_control();//委托类型对其他字段的控制
            }
        },
        allowBlank: false,
        blankText: '委托类型不能为空!'
    })
    //申报方式
    var store_REPWAYNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbfs
    })
    var combo_REPWAYNAME = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_REPWAYNAME',
        name: 'REPWAYID',
        store: store_REPWAYNAME,
        hideTrigger: true,
        fieldLabel: '申报方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 3,
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }
        },
        allowBlank: false,
        blankText: '申报方式不能为空!'
    })
    //申报关区
    var store_CUSTOMDISTRICTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.ComboBox', {//申报关区 这个数据比较多需要根据输入字符到后台动态模糊匹配
        name: 'CUSTOMDISTRICTCODE',
        store: store_CUSTOMDISTRICTNAME,
        fieldLabel: '申报关区',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        minChars: 2,
        hideTrigger: true,
        forceSelection: true,
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
            }
        },
        tabIndex: 4,
        allowBlank: false,
        blankText: '申报关区不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    var field_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMDISTRICTNAME'
    })
    //报关申报单位
    var tf_bgsbdw = Ext.create('Ext.form.field.Text', {
        id: 'tf_bgsbdw',
        readOnly: true,
        name: 'REPUNITCODE',
        margin: 0,
        flex: .85,
    })
    var cont_bgsbdw = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bgsbdw',
        fieldLabel: '报关申报单位',
        layout: 'hbox',
        items: [tf_bgsbdw, {
            id: 'bgsbdw_btn', xtype: 'button', handler: function () { bgsbdw_win(tf_bgsbdw); },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //报关方式
    var store_DECLWAY = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bgfs
    })
    var combo_DECLWAY = Ext.create('Ext.form.field.ComboBox', {
        name: 'DECLWAY',
        hideTrigger: true,
        store: store_DECLWAY,
        fieldLabel: '报关方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 6,
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }
        },
        allowBlank: false,
        blankText: '报关方式不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    //委托人员
    var field_SUBMITUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERNAME',
        fieldLabel: '委托人员',
        readOnly: true
    });
    //委托时间
    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITTIME',
        fieldLabel: '委托时间',
        readOnly: true
    });
    //委托电话
    var field_SUBMITUSERPHONE = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERPHONE',
        fieldLabel: '委托电话',
        readOnly: true
    })
    //报检申报单位
    var tf_bjsbdw = Ext.create('Ext.form.field.Text', {
        id: 'tf_bjsbdw',
        readOnly: true,
        name: 'INSPUNITCODE',
        margin: 0,
        flex: .85,
    })
    var cont_bjsbdw = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bjsbdw',
        fieldLabel: '报检申报单位',
        layout: 'hbox',
        items: [tf_bjsbdw, {
            id: 'bjsbdw_btn', xtype: 'button', handler: function () { bjsbdw_win(tf_bjsbdw); },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //维护人员
    var field_CREATEUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'CREATEUSERNAME',
        fieldLabel: '维护人员',
        readOnly: true
    });
    //维护时间
    var field_CREATETIME = Ext.create('Ext.form.field.Text', {
        name: 'CREATETIME',
        fieldLabel: '维护时间',
        readOnly: true
    });
    //平台客服
    var field_CSNAME = Ext.create('Ext.form.field.Text', {
        name: 'CSNAME',
        fieldLabel: '平台客服',
        readOnly: true
    });
    //客服电话
    var field_CSPHONE = Ext.create('Ext.form.field.Text', {
        name: 'CSPHONE',
        fieldLabel: '客服电话',
        readOnly: true
    });
    //业务状态
    var store_status = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: orderstatus_js_data
    })
    var field_STATUS = Ext.create('Ext.form.field.ComboBox', {//业务状态
        id: 'field_STATUS',
        name: 'STATUS',
        valueField: 'CODE',
        displayField: 'NAME',
        fieldLabel: '业务状态',
        queryMode: 'local',
        editable: false,
        hiddenTrigger: true,
        readOnly: true,
        labelWidth: 80,
        value: 1,
        store: store_status
    });
    var label_busiinfo = {
        columnWidth: .70,
        xtype: 'label',
        margin: '0 0 5 5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;业务信息</span></h4>'
    }
    var chk_2 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '价格影响确认',
        tabIndex: -1,
        readOnly: true,
        name: 'PRICEIMPACT'
    });
    var chk_spe_relation = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '特殊关系确认',
        tabIndex: -1,
        name: 'SPECIALRELATIONSHIP',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    chk_2.setReadOnly(false);
                } else {
                    chk_2.setValue(false);
                    chk_2.setReadOnly(true);
                }
            }
        }
    });
    var chk_3 = Ext.create('Ext.form.field.Checkbox', {
        labelWidth: 125,
        fieldLabel: '支付特许权使用费确认',
        tabIndex: -1,
        name: 'PAYPOYALTIES'
    });
    var chk_container = {
        columnWidth: .30,
        border: 2,
        height: 25,
        style: {
            borderColor: '#e9b477',
            borderStyle: 'solid'
        },
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [chk_spe_relation, chk_2, chk_3]
    }
    //客户编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        name: 'CUSNO',
        id: 'CUSNO',
        tabIndex: 8,
        fieldLabel: '客户编号'
    });
    //出口口岸
    var store_PORTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_PORTCODE = Ext.create('Ext.form.field.ComboBox', {//和申报关区一样 这个数据比较多需要根据输入字符到后台动态模糊匹配
        name: 'PORTCODE',
        store: store_PORTNAME,
        fieldLabel: '出口口岸',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        minChars: 2,
        hideTrigger: true,
        anyMatch: true,
        forceSelection: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
            },
            select: function (records) { field_PORTNAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('('))); }
        },
        tabIndex: 9,
        allowBlank: false,
        blankText: '出口口岸不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    var field_PORTNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'PORTNAME'
    })
    store_jydw = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'QUANCODE', 'QUANNAME', 'SHORTNAME'],
        data: common_data_jydw
    })
    //经营单位 这个数据比较多需要根据输入字符到后台动态模糊匹配,如果取不到点击添加按钮从总库进行选择，同时添加到自有客户库
    var combo_jydw = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_jydw',
        name: 'BUSIUNITCODE',
        store: store_jydw,
        displayField: 'NAME',
        valueField: 'QUANCODE',
        queryMode: 'local',
        minChars: 2,
        margin: 0,
        forceSelection: true,
        tabIndex: 10,
        anyMatch: true,
        hideTrigger: true,
        listeners: {
            select: function (combo, records) {
                field_BUSIUNITNAME.setValue(records[0].get("QUANNAME"));
                field_BUSISHORTCODE.setValue(records[0].get("CODE"));
                field_BUSISHORTNAME.setValue(records[0].get("SHORTNAME"));
            },
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
            }
        },
        flex: .85,
        allowBlank: false,
        blankText: '经营单位不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    //经营单位
    var field_jydw = {
        xtype: 'fieldcontainer',
        fieldLabel: '经营单位',
        layout: 'hbox',
        items: [combo_jydw, {
            id: 'jydw_btn', xtype: 'button', handler: function () {
                selectjydw(combo_jydw, field_BUSIUNITNAME, field_BUSISHORTCODE, field_BUSISHORTNAME);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    var field_BUSIUNITNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'BUSIUNITNAME'
    })
    var field_BUSISHORTCODE = Ext.create('Ext.form.field.Hidden', {
        id: 'field_BUSISHORTCODE',
        name: 'BUSISHORTCODE'
    })
    var field_BUSISHORTNAME = Ext.create('Ext.form.field.Hidden', {
        id: 'field_BUSISHORTNAME',
        name: 'BUSISHORTNAME'
    })
    //包装combostore
    var store_PACKKINDNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bzzl
    })
    var combo_PACKKINDNAME = Ext.create('Ext.form.field.ComboBox', {
        name: 'PACKKIND',
        hideTrigger: true,
        store: store_PACKKINDNAME,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 12,
        queryMode: 'local',
        anyMatch: true,
        margin: 0,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }
        },
        flex: .5,
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    //件数/包装
    var field_quanpackage = {
        xtype: 'fieldcontainer',
        fieldLabel: '件数/包装',
        layout: 'hbox',
        items: [{
            id: 'GOODSNUM', name: 'GOODSNUM', xtype: 'numberfield', tabIndex: 11, flex: .5, margin: 0, hideTrigger: true, allowBlank: false, blankText: '不能为空!'
        }, combo_PACKKINDNAME]
    }
    //毛重/净重
    var field_weight = {
        xtype: 'fieldcontainer',
        fieldLabel: '毛重/净重',
        layout: 'hbox',
        items: [
            {
                id: 'GOODSGW', name: 'GOODSGW', xtype: 'numberfield', flex: .5, msgTarget: 'qtip', tabIndex: 13, margin: 0, allowBlank: false, blankText: '不能为空!', hideTrigger: true, vtype: 'gwnwrule', decimalPrecision: 4
            },
           {
               id: 'GOODSNW', name: 'GOODSNW', xtype: 'numberfield', flex: .5, msgTarget: 'qtip', tabIndex: 14, margin: 0, hideTrigger: true, vtype: 'gwnwrule', decimalPrecision: 4
           }]
    }
    //合同号
    var field_contractno = Ext.create('Ext.form.field.Text', {
        tabIndex: 15,
        fieldLabel: '合同号',
        name: 'CONTRACTNO'
    });
    //船名/航次
    var field_ship = {
        xtype: 'fieldcontainer',
        fieldLabel: '船名/航次',
        layout: 'hbox',
        items: [
            {
                id: 'SHIPNAME', name: 'SHIPNAME', xtype: 'textfield', flex: .5, tabIndex: 16, margin: 0,
                allowBlank: false, blankText: '不能为空!', listeners: {
                    focus: function (tf, The, options) {
                        tf.clearInvalid();
                    }
                }
            },
            {
                id: 'FILGHTNO', name: 'FILGHTNO', xtype: 'textfield', flex: .5, tabIndex: 17, margin: 0,
                allowBlank: false, blankText: '不能为空!', listeners: {
                    focus: function (tf, The, options) {
                        tf.clearInvalid();
                    }
                }
            }]
    }
    //提单号
    var field_LADINGBILLNO = Ext.create('Ext.form.field.Text', {
        name: 'SECONDLADINGBILLNO',
        tabIndex: 18,
        fieldLabel: '提单号'
    });
    //贸易方式combostore
    var store_myfs = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    })
    var combo_myfs = Ext.create('Ext.form.field.ComboBox', {//贸易方式
        id: 'combo_myfs',
        store: store_myfs,
        displayField: 'CODE',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        submitValue: false,//不随表单提交一起提交
        tabIndex: 19,
        hideTrigger: true,
        multiSelect: false,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            },
            select: function (combo, records, eOpts) {
                field_TRADEWAYCODES.setValue(combo.rawValue.substr(0, 4));
                field_TRADEWAYCODES1.setValue(combo.rawValue.substr(0, 4));
            }
        },
        allowBlank: false,
        blankText: '贸易方式不能为空!',
        flex: 0.85,
        margin: 0,
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    var field_TRADEWAYCODES = Ext.create('Ext.form.field.Hidden', {
        name: 'TRADEWAYCODES'
    });
    var field_TRADEWAYCODES1 = Ext.create('Ext.form.field.Hidden', {
        name: 'TRADEWAYCODES1'
    });
    //贸易方式
    var field_myfs = {
        xtype: 'fieldcontainer',
        fieldLabel: '贸易方式',
        layout: 'hbox',
        items: [combo_myfs, {
            xtype: 'button', id: 'myfs_btn', handler: function () {
                selectmyfs(combo_myfs, field_TRADEWAYCODES, field_TRADEWAYCODES1);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //运抵编号
    var field_ARRIVEDNO = Ext.create('Ext.form.field.Text', {
        name: 'ARRIVEDNO',
        tabIndex: 20,
        maxLength: 18,
        minLength: 18,
        emptyText: '请填写18位运抵编号',
        fieldLabel: '运抵编号'
    });
    //货物类型
    var store_GOODSTYPEID = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: hwlx_js_data
    })
    var combo_GOODSTYPEID = Ext.create('Ext.form.field.ComboBox', {
        name: 'GOODSTYPEID',
        store: store_GOODSTYPEID,
        fieldLabel: '货物类型',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
        tabIndex: 21,
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }
        },
        allowBlank: false,
        blankText: '货物类型不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    //法检状况
    var chk_CHKLAWCONDITION = {
        xtype: 'checkboxfield',
        tabIndex: 22,
        fieldLabel: '法检状况',
        name: 'LAWCONDITION'
    }
    //通关单号
    var field_CLEARANCENO = Ext.create('Ext.form.field.Text', {
        id: 'field_CLEARANCENO',
        tabIndex: 23,
        fieldLabel: '通关单号',
        name: 'CLEARANCENO'
    });
    //集装箱号
    var field_CONTAINERNO = Ext.create('Ext.form.field.Text', {
        name: 'CONTAINERNO',
        id: 'CONTAINERNO',
        tabIndex: 24,
        readOnly: true,
        allowBlank: true,
        margin: 0,
        flex: 0.85
    })
    var container_box = Ext.create('Ext.form.FieldContainer', {
        id: 'fieldcontainer1',
        fieldLabel: '集装箱号',
        layout: 'hbox',
        items: [field_CONTAINERNO, {
            xtype: 'button', id: 'container_btn', handler: function () {
                win_container_truck.show();
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //集装箱号和报关车号有可能会有多个,保存时以数组的形式存储到该字段
    var field_CONTAINERTRUCK = Ext.create('Ext.form.field.Hidden', {
        id: 'field_CONTAINERTRUCK',
        name: 'CONTAINERTRUCK'
    })
    var field_bgch = Ext.create('Ext.form.field.Text', {
        name: 'DECLCARNO',
        id: 'DECLCARNO',
        tabIndex: 25,
        readOnly: true,
        allowBlank: true,
        margin: 0,
        flex: 0.85
    })
    var container_bgch = Ext.create('Ext.form.FieldContainer', {
        id: 'fieldbgch',
        fieldLabel: '报关车号',
        layout: 'hbox',
        items: [field_bgch, {
            id: 'bgch_button',
            xtype: 'button', id: 'declcarno_btn', listeners: {
                click: function () {
                    win_container_truck.show();
                }
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //转关预录号
    var field_TURNPRENO = Ext.create('Ext.form.field.Text', {
        fieldLabel: '转关预录号',
        name: 'TURNPRENO',
        readOnly: true
    });
    //需求备注
    var field_CLEARREMARK = Ext.create('Ext.form.field.Text', {
        id: 'field_ENTRUSTREQUEST',
        tabIndex: 26,
        fieldLabel: '需求备注',
        name: 'ENTRUSTREQUEST'
    });
    //隐藏的字段
    var field_ID = Ext.create('Ext.form.field.Hidden', {
        name: 'ID'
    });
    var field_BUSIKIND = Ext.create('Ext.form.field.Hidden', {
        name: 'BUSIKIND',
        value: '001'
    });
    var field_ORDERWAY = Ext.create('Ext.form.field.Hidden', {
        name: 'ORDERWAY',
        value: '1'
    });
    var field_CLEARUNIT = Ext.create('Ext.form.field.Hidden', {
        name: 'CLEARUNIT'
    });
    var field_CLEARUNITNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'CLEARUNITNAME'
    });
    var field_CUSTOMERCODE = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMERCODE'
    });
    var field_CUSTOMERNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMERNAME'
    });
    var field_ORIGINALFILEIDS = Ext.create('Ext.form.field.Hidden', {
        id: 'field_ORIGINALFILEIDS',
        name: 'ORIGINALFILEIDS'
    });
    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button onclick="orderBack();" id="btn_cancelsubmit" class="btn btn-primary btn-sm"><i class="fa fa-angle-double-left"></i>&nbsp;撤单</button>'
                        + '<button onclick="add_new(20)" class="btn btn-primary btn-sm"><i class="fa fa-plus fa-fw"></i>&nbsp;新增</button>'
                        + '<button type="button" onclick="copyorder(20)" class="btn btn-primary btn-sm"><i class="fa fa-files-o"></i>&nbsp;复制新增</button>'
                        + '<button onclick="save(\'save\',20)" class="btn btn-primary btn-sm"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
                        + '<button onclick="save(\'submit\',20)" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;提交委托</button></div>'

    var bbar_l = '<div class="btn-group">'
               + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
               + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
               + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
           + '</div>';
    var store_filetype = Ext.create('Ext.data.JsonStore', {
        fields: ['FILETYPEID', 'FILETYPENAME'],
        data: [{ FILETYPEID: '44', FILETYPENAME: '订单文件' },
               { FILETYPEID: '58', FILETYPENAME: '配舱单文件' }]

    })
    var combo_filetype = Ext.create('Ext.form.field.ComboBox', {//文件类型
        id: 'combo_filetype',
        name: 'FILETYPEID',
        store: store_filetype,
        fieldLabel: '文件类型',
        displayField: 'FILETYPENAME',
        valueField: 'FILETYPEID',
        editable: false,
        queryMode: 'local',
        labelWidth: 60,
        width: 150,
        value: '44'
    })
    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [combo_filetype, bbar_l, '->', bbar_r]
    })
    formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        minHeight: 350,
        border: 0,
        tbar: tbar,
        bbar: bbar,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .20,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [
        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_CODE, combo_ENTRUSTTYPENAME, combo_REPWAYNAME, combo_CUSTOMDISTRICTNAME, cont_bgsbdw] },
        { layout: 'column', height: 42, border: 0, items: [combo_DECLWAY, field_SUBMITUSERNAME, field_SUBMITTIME, field_SUBMITUSERPHONE, cont_bjsbdw] },
        { layout: 'column', height: 42, border: 0, items: [field_CREATEUSERNAME, field_CREATETIME, field_CSNAME, field_CSPHONE, field_STATUS] },
        { layout: 'column', border: 42, border: 0, items: [label_busiinfo, chk_container] },
        { layout: 'column', height: 42, border: 0, items: [field_CUSNO, combo_PORTCODE, field_jydw, field_quanpackage, field_weight] },
        { layout: 'column', height: 42, border: 0, items: [field_contractno, field_ship, field_LADINGBILLNO, field_myfs, field_ARRIVEDNO] },
        { layout: 'column', height: 42, border: 0, items: [combo_GOODSTYPEID, chk_CHKLAWCONDITION, field_CLEARANCENO, container_box, container_bgch] },
        { layout: 'column', height: 42, border: 0, items: [field_TURNPRENO, field_CLEARREMARK] },
        field_CUSTOMDISTRICTNAME, field_PORTNAME, field_BUSIUNITNAME, field_BUSISHORTCODE, field_BUSISHORTNAME, field_CUSTOMERCODE, field_CUSTOMERNAME,
        field_ID, field_BUSIKIND, field_ORDERWAY, field_CLEARUNIT, field_CLEARUNITNAME, field_TRADEWAYCODES, field_TRADEWAYCODES1,
        field_CONTAINERTRUCK, field_ORIGINALFILEIDS
        ]
    });
}