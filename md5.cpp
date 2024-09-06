#include "ui_md5.h"
#include <md5.h>
#include <QCryptographicHash>
#include <QClipboard>
#include <QMessageBox>
#include <QTimer>
#include <qtimer.h>

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

void md5::on_submit_button_clicked(QAbstractButton *button)
{
}

void md5::on_submit_button_accepted()
{
    QString text = ui->source_text->toPlainText();
    if (!text.isEmpty()) {
        QByteArray qba = QCryptographicHash::hash(text.toLatin1(), QCryptographicHash::Md5);
        QString textMd5 = qba.toHex();
        ui->upper32_text->setText(textMd5.toUpper());
        ui->lower32_text->setText(textMd5.toLower());
        QString textMd5_16 = textMd5.mid(8, 16);
        ui->upper16_text->setText(textMd5_16.toUpper());
        ui->lower16_text->setText(textMd5_16.toLower());
    }
}


void md5::on_submit_button_rejected()
{
    ui->source_text->setText("");
    ui->upper32_text->setText("");
    ui->lower32_text->setText("");
    ui->upper16_text->setText("");
    ui->lower16_text->setText("");
}


void md5::on_upper32_copy_clicked()
{
    QString text = ui->upper32_text->text();
    if (!text.isEmpty()) {
        QClipboard *clipboard = QGuiApplication::clipboard();
        clipboard->setText(text);
        alert("Copied");
    }
}

void md5::alert(QString text)
{
    QMessageBox *mbox = new QMessageBox;
    mbox->setWindowTitle(text);
    mbox->setText(text);
    mbox->show();
    QTimer::singleShot(1000, mbox, SLOT(hide()));
}


void md5::on_lower32_copy_clicked()
{
    QString text = ui->lower32_text->text();
    if (!text.isEmpty()) {
        QClipboard *clipboard = QGuiApplication::clipboard();
        clipboard->setText(text);
        alert("Copied");
    }
}


void md5::on_upper16_copy_clicked()
{
    QString text = ui->upper16_text->text();
    if (!text.isEmpty()) {
        QClipboard *clipboard = QGuiApplication::clipboard();
        clipboard->setText(text);
        alert("Copied");
    }
}


void md5::on_lower16_copy_clicked()
{
    QString text = ui->lower16_text->text();
    if (!text.isEmpty()) {
        QClipboard *clipboard = QGuiApplication::clipboard();
        clipboard->setText(text);
        alert("Copied");
    }
}

