#include "md5.h"
#include "ui_md5.h"

md5::md5(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::md5)
{
    ui->setupUi(this);
}

md5::~md5()
{
    delete ui;
}

void md5::on_button_accepted()
{
}
