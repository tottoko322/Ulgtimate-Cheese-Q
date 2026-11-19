# 共通の攻撃判定・食らい判定

## 配置

既存の身体用Colliderは残し、判定ごとに子GameObjectを追加します。
各子にはCollider2Dを一つだけ付けます。判定用ColliderはTriggerとして使用します。
持ち主にはRigidbody2D、Damageable、IDamageReceiverを実装したControllerが必要です。

- BodyHurtbox: HurtboxとBoxCollider2Dなど。Starts Enabledをオン。
- LimbHurtbox / WeaponHurtbox: HurtboxとCollider2D。Starts Enabledをオフ。
- AttackHitbox: HitboxとCollider2D。Target Layersに相手のHurtbox用レイヤーを設定。

ReceiverとOwnerは親のDamageableを自動取得します。別階層の武器などはInspectorから指定します。
食らい判定用レイヤーは、地形・身体衝突用レイヤーと分けることを推奨します。
食らい判定同士、攻撃判定同士の接触からダメージは発生しません。

## 攻撃

HitboxのAttack Dataに攻撃ごとの値を設定します。
攻撃発生時にBeginAttack(-1)またはBeginAttack(1)、終了時にEndAttack()を呼びます。
方向は攻撃開始時に固定します。Xノックバックだけ左右反転し、Yは設定値を保持します。
Colliderの位置も持ち主の向きに合わせてください。引数だけでは範囲の位置は反転しません。

一つの攻撃は一つのHitboxで管理します。攻撃窓は最低一回の物理更新を含めてください。
同じ攻撃で胴体と武器の両方に接触しても、持ち主一体に対する通知は一回です。
無敵で拒否された場合も、その攻撃では再通知しません。次のBeginAttackで記録をリセットします。
攻撃中断・被弾・死亡時にもEndAttackを呼びます。
無効化・再有効化では攻撃を自動再開しません。

攻撃中の手足や武器はSetReceiving(true/false)で食らい判定を独立に切り替えます。
自分自身のHitboxで自分のHurtboxへダメージを与えることはありません。

## 既存処理との接続

現在のControllerのOverlapCircleとデバッグ被弾は、この判定を使う攻撃について停止してください。
同じ攻撃から両方を呼ぶと二重通知になります。
DamageableとIDamageReceiverの既存APIは変更していません。
回避の無敵やHurt・Dead中の移動制御は別途Controller側で接続する必要があります。

## 動作確認

- Player / Enemyの基本攻撃で互いのHPが減る。
- 胴体と手足へ同時に接触しても一回分だけ減る。
- 二回目の攻撃では再び命中する。
- 左右でXノックバックのみ反転し、Yは同じ。
- 無効な食らい判定、自分自身、対象外レイヤーには当たらない。
- 接触したまま食らい判定を有効にしても、次の物理更新で判定される。
- 攻撃を中断・無効化すると、それ以降命中しない。

シーン・Prefabと各Controllerの接続は未変更です。配置後にUnityで確認してください。
