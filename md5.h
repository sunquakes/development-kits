#ifndef MD5_H
#define MD5_H

#include <QWidget>

namespace Ui {
class md5;
}

class md5 : public QWidget
{
    Q_OBJECT

public:
    explicit md5(QWidget *parent = nullptr);
    ~md5();

private:
    Ui::md5 *ui;
};

#endif // MD5_H
