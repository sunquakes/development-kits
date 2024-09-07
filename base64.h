#ifndef BASE64_H
#define BASE64_H

#include <QWidget>

namespace Ui {
class Base64;
}

class Base64 : public QWidget
{
    Q_OBJECT

public:
    explicit Base64(QWidget *parent = nullptr);
    ~Base64();

private slots:
    void on_encodeButton_clicked();

    void on_decodeButton_clicked();

    void on_clearButton_clicked();

private:
    Ui::Base64 *ui;
};

#endif // BASE64_H
