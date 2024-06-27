#ifndef MD5_H
#define MD5_H

#include <QWidget>
#include <qabstractbutton.h>

namespace Ui {
class md5;
}

class md5 : public QWidget
{
    Q_OBJECT

public:
    explicit md5(QWidget *parent = nullptr);
    ~md5();
private slots:
    void on_button_accepted();

    void on_submit_button_clicked(QAbstractButton *button);

    void on_submit_button_accepted();

    void on_submit_button_rejected();

    void on_upper32_copy_clicked();

    void alert(QString text);

    void on_lower32_copy_clicked();

    void on_upper16_copy_clicked();

    void on_lower16_copy_clicked();

private:
    Ui::md5 *ui;
};

#endif // MD5_H
